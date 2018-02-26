# Find max elevation change between sequentially adjacent gps coordinates collected 
select r.*, r.next_distance - r.elevation as climbed
from (select r.*,
             (select elevation
              from gps_coordinate_log r2
              where r2.elevation > r.elevation
              order by time_collected
              limit 1
             ) as next_distance
      from gps_coordinate_log r 
     ) r where next_distance is not null 
order by climbed desc;


