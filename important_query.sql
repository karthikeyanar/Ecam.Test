select  concat('{\'symbol\':\'',symbol,'\',\'type\':\'EQ\'},') as q from (
select count(m.symbol) as cnt,c.symbol from tra_company c
left outer join tra_market m on m.symbol = c.symbol and m.trade_date <= '2017-12-31'
group by c.symbol
) as tbl  where cnt <= 0 order by cnt asc,symbol asc
limit 0,1000