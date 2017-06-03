ALTER TABLE `tra_company` ADD COLUMN `high_count` INTEGER(11) DEFAULT NULL;
ALTER TABLE `tra_company` ADD COLUMN `low_count` INTEGER(11) DEFAULT NULL;
CREATE TABLE `tra_intra_day_profit` (
  `symbol` VARCHAR(50) COLLATE utf8_general_ci NOT NULL,
  `trade_date` DATE NOT NULL,
  `first_percentage` DECIMAL(13,4) DEFAULT NULL,
  `last_percentage` DECIMAL(13,4) DEFAULT NULL,
  `profit_percentage` DECIMAL(13,4) DEFAULT NULL,
  `reverse_percentage` DECIMAL(13,4) DEFAULT NULL,
  `final_percentage` DECIMAL(13,4) DEFAULT NULL,
  `high_count` INTEGER(11) DEFAULT NULL,
  `low_count` INTEGER(11) DEFAULT NULL,
  PRIMARY KEY (`symbol`, `trade_date`) USING BTREE
) ENGINE=InnoDB
CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'
;