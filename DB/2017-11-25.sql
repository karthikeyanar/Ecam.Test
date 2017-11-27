ALTER TABLE `tra_company` CHANGE COLUMN `is_current_stock` `is_book_mark` BIT(1) DEFAULT NULL;
delete from tra_company_category where category_name = '2_FAVOURITES';
delete from tra_company_category where category_name = '1_CURRENT_STOCKS';