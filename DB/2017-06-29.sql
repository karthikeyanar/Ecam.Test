ALTER TABLE `tra_market_intra_day` ADD COLUMN `rsi` DECIMAL(13,4) DEFAULT NULL;
ALTER TABLE `tra_intra_day_profit` ADD COLUMN `rsi` DECIMAL(13,4) DEFAULT NULL;
ALTER TABLE `tra_intra_day_profit` ADD COLUMN `prev_rsi` DECIMAL(13,4) DEFAULT NULL;
ALTER TABLE `tra_intra_day_profit` ADD COLUMN `diff_rsi` DECIMAL(13,4) DEFAULT NULL;
ALTER TABLE `tra_company` DROP COLUMN `day_5`;
ALTER TABLE `tra_company` DROP COLUMN `day_10`;
ALTER TABLE `tra_company` DROP COLUMN `day_15`;
ALTER TABLE `tra_company` DROP COLUMN `day_20`;
ALTER TABLE `tra_company` DROP COLUMN `day_25`;
ALTER TABLE `tra_company` DROP COLUMN `day_30`;
ALTER TABLE `tra_company` DROP COLUMN `day_35`;
ALTER TABLE `tra_company` DROP COLUMN `day_60`;
ALTER TABLE `tra_company` DROP COLUMN `day_40`;
ALTER TABLE `tra_company` DROP COLUMN `day_45`;
ALTER TABLE `tra_company` DROP COLUMN `day_50`;
ALTER TABLE `tra_company` DROP COLUMN `day_55`;
ALTER TABLE `tra_company` DROP COLUMN `day_65`;
ALTER TABLE `tra_company` DROP COLUMN `day_70`;
ALTER TABLE `tra_company` DROP COLUMN `day_75`;
ALTER TABLE `tra_company` DROP COLUMN `day_80`;
ALTER TABLE `tra_company` DROP COLUMN `day_85`;
ALTER TABLE `tra_company` DROP COLUMN `day_90`;
ALTER TABLE `tra_company` DROP COLUMN `mf_cnt`;
ALTER TABLE `tra_company` DROP COLUMN `mf_qty`;
ALTER TABLE `tra_company` DROP COLUMN `day_2`;
ALTER TABLE `tra_company` DROP COLUMN `day_3`;
ALTER TABLE `tra_company` DROP COLUMN `day_4`;
ALTER TABLE `tra_company` DROP COLUMN `day_1`;
ALTER TABLE `tra_company` DROP COLUMN `high_count`;
ALTER TABLE `tra_company` DROP COLUMN `low_count`;
ALTER TABLE `tra_intra_day_profit` DROP COLUMN `high_count`;
ALTER TABLE `tra_intra_day_profit` DROP COLUMN `low_count`;
