/* =====================================================
   UNIVERSAL MIGRATION for sostoyanie (M2M)
   All comparisons forced to utf8mb4_0900_ai_ci
   ===================================================== */


/* ===== БЛОК 1. SOSTOYANIE → словарь + M2M ===== */
START TRANSACTION;

CREATE TABLE IF NOT EXISTS `sostoyanie` (
  `id`   INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_sostoyanie_name` (`name`)
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `catalog_sostoyanie` (
  `catalog_id`    INT NOT NULL,
  `sostoyanie_id` INT NOT NULL,
  PRIMARY KEY (`catalog_id`, `sostoyanie_id`),
  KEY `ix_sostoyanie` (`sostoyanie_id`),
  CONSTRAINT `fk_cs_catalog`  FOREIGN KEY (`catalog_id`)    REFERENCES `catalog`(`id`)      ON DELETE CASCADE,
  CONSTRAINT `fk_cs_spr_sost` FOREIGN KEY (`sostoyanie_id`) REFERENCES `sostoyanie`(`id`)   ON DELETE RESTRICT
) ENGINE=InnoDB;

/* временная таблица токенов */
DROP TEMPORARY TABLE IF EXISTS `tmp_catalog_tokens`;
CREATE TEMPORARY TABLE `tmp_catalog_tokens` (
  `catalog_id` INT NOT NULL,
  `token`      VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL
) ENGINE=Memory;

/* парсинг catalog.sostoyanie */
INSERT INTO `tmp_catalog_tokens` (`catalog_id`, `token`)
WITH RECURSIVE split AS (
  SELECT
    c.id AS catalog_id,
    TRIM(BOTH ', ' FROM
      REPLACE(
        REPLACE(
          REPLACE(COALESCE(c.sostoyanie, ''), ';', ','),
        ' ,', ','),
      ',,', ',')
    ) AS rest,
    CAST(NULL AS CHAR(255)) AS token,
    0 AS lvl
  FROM `catalog` c

  UNION ALL

  SELECT
    catalog_id,
    CASE WHEN INSTR(rest, ',') = 0 THEN ''
         ELSE LTRIM(SUBSTRING(rest, INSTR(rest, ',') + 1))
    END AS rest,
    TRIM(
      CASE WHEN INSTR(rest, ',') = 0 THEN rest
           ELSE SUBSTRING(rest, 1, INSTR(rest, ',') - 1)
      END
    ) AS token,
    lvl + 1
  FROM split
  WHERE rest <> ''
)
SELECT catalog_id,
       REGEXP_REPLACE(token, ' +', ' ') AS token
FROM split
WHERE lvl > 0
  AND token IS NOT NULL
  AND token <> '';

/* наполняем словарь (DISTINCT по токенам) */
INSERT IGNORE INTO `sostoyanie`(`name`)
SELECT DISTINCT token
FROM `tmp_catalog_tokens`;

/* связи M2M: сравнение тоже через CONVERT ... USING utf8mb4 + COLLATE */
INSERT IGNORE INTO `catalog_sostoyanie` (`catalog_id`, `sostoyanie_id`)
SELECT t.catalog_id, s.id
FROM `tmp_catalog_tokens` t
JOIN `sostoyanie` s
  ON CONVERT(t.token USING utf8mb4) COLLATE utf8mb4_0900_ai_ci
   = CONVERT(s.name USING utf8mb4) COLLATE utf8mb4_0900_ai_ci;

/* убираем временную */
DROP TEMPORARY TABLE IF EXISTS `tmp_catalog_tokens`;

COMMIT;

/* ===== БЛОК 2. Слияние дублей в sostoyanie (точка и т.п.) ===== */
START TRANSACTION;

DROP TEMPORARY TABLE IF EXISTS `t_norm`;
CREATE TEMPORARY TABLE `t_norm` (
  `id`   INT NOT NULL,
  `name` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `norm` VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `has_trailing_punct` TINYINT(1) NOT NULL
) ENGINE=Memory;

INSERT INTO `t_norm` (`id`,`name`,`norm`,`has_trailing_punct`)
SELECT
  s.id,
  s.name,
  REGEXP_REPLACE(
    REGEXP_REPLACE(LOWER(TRIM(s.name)), ' +', ' '),
    '[[:punct:]]+$', ''
  ) AS norm,
  CASE WHEN TRIM(s.name) REGEXP '[[:punct:]]$' THEN 1 ELSE 0 END AS has_trailing_punct
FROM `sostoyanie` s;

DROP TEMPORARY TABLE IF EXISTS `t_keep`;
CREATE TEMPORARY TABLE `t_keep` (
  `norm`    VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `keep_id` INT NOT NULL,
  PRIMARY KEY(`norm`)
) ENGINE=Memory;

INSERT INTO `t_keep` (`norm`,`keep_id`)
SELECT norm, id
FROM (
  SELECT
    id,
    norm,
    ROW_NUMBER() OVER (
      PARTITION BY norm
      ORDER BY has_trailing_punct ASC, id ASC
    ) AS rn
  FROM `t_norm`
) x
WHERE rn = 1;

DROP TEMPORARY TABLE IF EXISTS `t_map`;
CREATE TEMPORARY TABLE `t_map` (
  `old_id`  INT NOT NULL,
  `keep_id` INT NOT NULL,
  PRIMARY KEY(`old_id`)
) ENGINE=Memory;

INSERT INTO `t_map` (`old_id`,`keep_id`)
SELECT n.id AS old_id, k.keep_id
FROM `t_norm` n
JOIN `t_keep` k
  ON CONVERT(n.norm USING utf8mb4) COLLATE utf8mb4_0900_ai_ci
   = CONVERT(k.norm USING utf8mb4) COLLATE utf8mb4_0900_ai_ci
WHERE n.id <> k.keep_id;

/* переносим связи на канон */
INSERT IGNORE INTO `catalog_sostoyanie` (`catalog_id`,`sostoyanie_id`)
SELECT cs.catalog_id, m.keep_id
FROM `catalog_sostoyanie` cs
JOIN `t_map` m ON m.old_id = cs.sostoyanie_id;

/* удаляем старые связи и дубликаты из словаря */
DELETE cs.*
FROM `catalog_sostoyanie` cs
JOIN `t_map` m ON m.old_id = cs.sostoyanie_id;

DELETE s.*
FROM `sostoyanie` s
JOIN `t_map` m ON m.old_id = s.id;

COMMIT;

/* чистим временные */
DROP TEMPORARY TABLE IF EXISTS `t_map`;
DROP TEMPORARY TABLE IF EXISTS `t_keep`;
DROP TEMPORARY TABLE IF EXISTS `t_norm`;

/* ===== Подсказки =====
-- Проверка whatremont: строки без сопоставления
-- SELECT id, `whatremont` FROM `catalog` WHERE `whatremont` IS NOT NULL AND `whatremont_id` IS NULL LIMIT 50;

-- Проверка sostoyanie:
-- SELECT c.id, GROUP_CONCAT(s.name ORDER BY s.name)
-- FROM catalog c
-- LEFT JOIN catalog_sostoyanie cs ON cs.catalog_id=c.id
-- LEFT JOIN sostoyanie s ON s.id=cs.sostoyanie_id
-- GROUP BY c.id LIMIT 20;

-- Когда всё ок:
-- ALTER TABLE `catalog` MODIFY `whatremont_id` INT NOT NULL;
-- (и при полном отказе от текстов)
-- ALTER TABLE `catalog` DROP COLUMN `whatremont`;
-- ALTER TABLE `catalog` DROP COLUMN `sostoyanie`;
*/