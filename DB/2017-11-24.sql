ALTER TABLE `tra_company` CHANGE COLUMN `is_book_mark` `is_archive` BIT(1) DEFAULT NULL;
update tra_company set is_archive = 0;