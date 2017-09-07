ALTER TABLE `tra_market` MODIFY COLUMN `market_id` INTEGER(11) NOT NULL;
ALTER TABLE `tra_market` DROP INDEX `PRIMARY`;
ALTER TABLE `tra_market` MODIFY COLUMN `market_id` INTEGER(11) DEFAULT NULL;
ALTER TABLE `tra_market` DROP COLUMN `market_id`;
ALTER TABLE `tra_market` DROP COLUMN `trade_type`;
ALTER TABLE `tra_market` MODIFY COLUMN `trade_date` DATE NOT NULL;
