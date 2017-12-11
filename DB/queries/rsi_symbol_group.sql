select * from (select symbol
,(select count(*) as cnt from tra_rsi_profit where symbol = rsi.symbol and ifnull(profit,0)<=0) as fail
,(select count(*) as cnt from tra_rsi_profit where symbol = rsi.symbol and ifnull(profit,0)>0) as success 
from tra_rsi_profit rsi
group by symbol ) as tbl order by success desc limit 0,1000