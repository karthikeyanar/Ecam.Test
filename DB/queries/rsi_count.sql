select count(*) as cnt from tra_rsi_profit where ifnull(profit,0)<=0 order by profit asc limit 0,100
select count(*) as cnt from tra_rsi_profit where ifnull(profit,0)>0 order by profit asc limit 0,100
