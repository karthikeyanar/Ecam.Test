select * from (select distinct cc.symbol from tra_company_category cc 
join tra_category c on c.category_name = cc.category_name
where c.is_book_mark = 1) as tbl order by symbol  limit 0,1000