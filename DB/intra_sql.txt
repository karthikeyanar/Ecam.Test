select tbl.*
,(((ifnull(tbl.ltp_price, 0) - ifnull(tbl.prev_price, 0)) / ifnull(tbl.prev_price, 0)) * 100) as prev_percentage
from (select intra.symbol
,intra.trade_date
,DATE_FORMAT(intra.trade_date,'%h:%i %p') as time 
,c.open_price
,intra.ltp_price
,(select ltp_price from tra_market_intra_day where symbol = intra.symbol and trade_date < intra.trade_date order by trade_date desc limit 0,1) as prev_price
,(((ifnull(intra.ltp_price, 0) - ifnull(c.open_price, 0)) / ifnull(c.open_price, 0)) * 100) as ltp_percentage
from tra_market_intra_day intra
join tra_company c on c.symbol = intra.symbol
where intra.symbol = 'RCOM' order by intra.trade_date asc
) as tbl