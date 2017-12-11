ALTER TABLE `tra_company` ADD COLUMN `is_old` BIT(1) DEFAULT NULL;
ALTER TABLE `tra_category` ADD COLUMN `is_archive` BIT(1) DEFAULT NULL;
ALTER TABLE `tra_category` ADD COLUMN `is_book_mark` BIT(1) DEFAULT NULL;
