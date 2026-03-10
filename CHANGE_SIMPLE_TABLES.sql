
-- whatremont

SET NAMES UTF8MB4;

START TRANSACTION;

-- 1) Справочник whatremont (если нет)
CREATE TABLE IF NOT EXISTS `whatremont` (
  `id`   INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_whatremont_name` (`name`)
) ENGINE=InnoDB
  AUTO_INCREMENT=1
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_general_ci;

-- 2) Добавляем catalog.whatremont_id ПОСЛЕ whatremont, если колонки ещё нет
SET @schema := DATABASE();
SELECT COUNT(*) INTO @col_exists
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = @schema
  AND TABLE_NAME   = 'catalog'
  AND COLUMN_NAME  = 'whatremont_id';

SET @ddl := IF(@col_exists = 0,
  'ALTER TABLE `catalog` ADD COLUMN `whatremont_id` INT NULL AFTER `whatremont`',
  'SELECT 1'
);
PREPARE stmt FROM @ddl; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 3) Наполняем справочник уникальными значениями из catalog.whatremont
INSERT IGNORE INTO `whatremont` (`name`)
SELECT DISTINCT
  TRIM(REGEXP_REPLACE(`whatremont`, ' +', ' '))
FROM `catalog`
WHERE `whatremont` IS NOT NULL
  AND TRIM(`whatremont`) <> '';

-- 4) Заполняем catalog.whatremont_id по нормализованному совпадению
UPDATE `catalog` c
JOIN `whatremont` w

  ON TRIM(c.whatremont) COLLATE utf8mb4_general_ci
   = TRIM(w.name) COLLATE utf8mb4_general_ci

SET c.`whatremont_id` = w.`id`
WHERE c.`whatremont` IS NOT NULL
  AND TRIM(c.`whatremont`) <> '';

-- 5) Индекс под FK (если ещё нет)
SET @idx_exists := (
  SELECT COUNT(*)
  FROM INFORMATION_SCHEMA.STATISTICS
  WHERE TABLE_SCHEMA = @schema
    AND TABLE_NAME   = 'catalog'
    AND INDEX_NAME   = 'ix_catalog_whatremont_id'
);
SET @ddl2 := IF(@idx_exists = 0,
  'ALTER TABLE `catalog` ADD INDEX `ix_catalog_whatremont_id`(`whatremont_id`)',
  'SELECT 1'
);
PREPARE stmt2 FROM @ddl2; EXECUTE stmt2; DEALLOCATE PREPARE stmt2;

-- 6) Внешний ключ
ALTER TABLE `catalog`
  ADD CONSTRAINT `FK_catalog_whatremont`
  FOREIGN KEY (`whatremont_id`)
  REFERENCES `whatremont`(`id`)
  ON DELETE CASCADE
  ON UPDATE RESTRICT;


ALTER TABLE `catalog`
  DROP COLUMN `whatremont`;


COMMIT;







SET NAMES UTF8MB4;

START TRANSACTION;

-- 1) Справочник моделей (если нет)
CREATE TABLE IF NOT EXISTS `model` (
  `id`   INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(120) CHARACTER SET utf8mb4 COLLATE UTF8MB4_GENERAL_CI NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_model_name` (`name`)
) ENGINE=InnoDB
  AUTO_INCREMENT=1
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_general_ci;

-- 2) Добавляем столбец catalog.model_id ПОСЛЕ model, если его нет
SET @schema := DATABASE();
SELECT COUNT(*) INTO @col_exists
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = @schema
  AND TABLE_NAME   = 'catalog'
  AND COLUMN_NAME  = 'model_id';

SET @ddl := IF(@col_exists = 0,
  'ALTER TABLE `catalog` ADD COLUMN `model_id` INT NULL AFTER `model`',
  'SELECT 1'
);
PREPARE stmt FROM @ddl; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 3) Переносим уникальные значения из catalog.model в справочник model
--    Нормализуем пробелы и регистр, чтобы не плодить дублей
INSERT IGNORE INTO `model` (`name`)
SELECT DISTINCT
  TRIM(REGEXP_REPLACE(`model`, ' +', ' '))
FROM `catalog`
WHERE `model` IS NOT NULL
  AND TRIM(`model`) <> '';

-- 4) Заполняем catalog.model_id по совпадению названий (нормализованное сравнение)
UPDATE `catalog` c
JOIN `model` m


  ON TRIM(c.model) COLLATE utf8mb4_general_ci
   = TRIM(m.name) COLLATE utf8mb4_general_ci

SET c.`model_id` = m.`id`
WHERE c.`model` IS NOT NULL
  AND TRIM(c.`model`) <> '';

-- 5) Индекс под внешний ключ (если ещё нет)
SET @idx_exists := (
  SELECT COUNT(*)
  FROM INFORMATION_SCHEMA.STATISTICS
  WHERE TABLE_SCHEMA = @schema
    AND TABLE_NAME   = 'catalog'
    AND INDEX_NAME   = 'ix_catalog_model_id'
);
SET @ddl2 := IF(@idx_exists = 0,
  'ALTER TABLE `catalog` ADD INDEX `ix_catalog_model_id`(`model_id`)',
  'SELECT 1'
);
PREPARE stmt2 FROM @ddl2; EXECUTE stmt2; DEALLOCATE PREPARE stmt2;

-- 6) Внешний ключ
ALTER TABLE `catalog`
  ADD CONSTRAINT `FK_catalog_model`
  FOREIGN KEY (`model_id`)
  REFERENCES `model`(`id`)
  ON DELETE CASCADE
  ON UPDATE RESTRICT;
  
  
ALTER TABLE `catalog`
  DROP COLUMN `model`;

  
COMMIT;








SET NAMES utf8mb4;

START TRANSACTION;

-- 1) Справочник брендов (если нет)
CREATE TABLE IF NOT EXISTS `brand` (
  `id`   INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_brand_name` (`name`)
) ENGINE=InnoDB
  AUTO_INCREMENT=1
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_general_ci;

-- 2) Добавляем столбец catalog.brand_id ПОСЛЕ brand, если его нет
SET @schema := DATABASE();
SELECT COUNT(*) INTO @col_exists
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = @schema
  AND TABLE_NAME   = 'catalog'
  AND COLUMN_NAME  = 'brand_id';

SET @ddl := IF(@col_exists = 0,
  'ALTER TABLE `catalog` ADD COLUMN `brand_id` INT NULL AFTER `brand`',
  'SELECT 1'
);
PREPARE stmt FROM @ddl; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 3) Переносим уникальные значения из catalog.brand в справочник brand
INSERT IGNORE INTO `brand` (`name`)
SELECT DISTINCT
  TRIM(REGEXP_REPLACE(`brand`, ' +', ' '))
FROM `catalog`
WHERE `brand` IS NOT NULL
  AND TRIM(`brand`) <> '';

-- 4) Заполняем catalog.brand_id по нормализованному совпадению
UPDATE `catalog` c
JOIN `brand` b


  ON TRIM(c.brand) COLLATE utf8mb4_general_ci
   = TRIM(b.name) COLLATE utf8mb4_general_ci

SET c.`brand_id` = b.`id`
WHERE c.`brand` IS NOT NULL
  AND TRIM(c.`brand`) <> '';

-- 5) Индекс под FK (если ещё нет)
SET @idx_exists := (
  SELECT COUNT(*)
  FROM INFORMATION_SCHEMA.STATISTICS
  WHERE TABLE_SCHEMA = @schema
    AND TABLE_NAME   = 'catalog'
    AND INDEX_NAME   = 'ix_catalog_brand_id'
);
SET @ddl2 := IF(@idx_exists = 0,
  'ALTER TABLE `catalog` ADD INDEX `ix_catalog_brand_id`(`brand_id`)',
  'SELECT 1'
);
PREPARE stmt2 FROM @ddl2; EXECUTE stmt2; DEALLOCATE PREPARE stmt2;

-- 6) Внешний ключ
ALTER TABLE `catalog`
  ADD CONSTRAINT `FK_catalog_brand`
  FOREIGN KEY (`brand_id`)
  REFERENCES `brand`(`id`)
  ON DELETE CASCADE
  ON UPDATE RESTRICT;

ALTER TABLE `catalog`
  DROP COLUMN `brand`;

COMMIT;

