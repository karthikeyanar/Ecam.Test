ALTER TABLE `tra_market_intra_day` ADD COLUMN `rsi` DECIMAL(13,4) DEFAULT NULL;
ALTER TABLE `tra_intra_day_profit` ADD COLUMN `rsi` DECIMAL(13,4) DEFAULT NULL;
ALTER TABLE `tra_intra_day_profit` ADD COLUMN `prev_rsi` DECIMAL(13,4) DEFAULT NULL;
ALTER TABLE `tra_intra_day_profit` ADD COLUMN `diff_rsi` DECIMAL(13,4) DEFAULT NULL;
