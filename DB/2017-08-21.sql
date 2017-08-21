CREATE TABLE `tra_market_avg` (
  `market_avg_id` INTEGER(11) NOT NULL AUTO_INCREMENT,
  `symbol` VARCHAR(50) COLLATE utf8_general_ci NOT NULL,
  `avg_type` VARCHAR(1) COLLATE utf8_general_ci NOT NULL,
  `avg_date` DATETIME(6) NOT NULL,
  `percentage` DECIMAL(13,4) NOT NULL,
  PRIMARY KEY (`market_avg_id`) USING BTREE,
  UNIQUE KEY `tra_market_avg_unique` (`symbol`, `avg_type`, `avg_date`) USING BTREE
) ENGINE=InnoDB
AUTO_INCREMENT=1 CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'
;
DROP TABLE `tra_intra_day_profit_calc`;
DROP TABLE `tra_pre_calc_item`;
DROP TABLE `tra_prev_calc`;
DROP TABLE `tra_intra_day_profit`;
DROP TABLE `tra_rsi_profit`;
ALTER TABLE `tra_company` ADD COLUMN `monthly_avg` DECIMAL(13,4) DEFAULT NULL;
ALTER TABLE `tra_company` ADD COLUMN `weekly_avg` DECIMAL(13,4) DEFAULT NULL;
