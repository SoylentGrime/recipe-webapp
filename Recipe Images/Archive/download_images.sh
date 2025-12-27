#!/usr/bin/env bash
set -euo pipefail

mkdir -p images

echo "Downloading: Kathy's Breakfast Casserole (Overnight)"
curl -L --fail --silent --show-error "https://www.cscassets.com/recipes/large_cknew/cknew_6718.jpg" -o "images/kathy-s-breakfast-casserole-overnight.jpg"

echo "Downloading: Blueberry Biscuits"
curl -L --fail --silent --show-error "https://www.mybakingaddiction.com/wp-content/uploads/2014/03/two-blueberry-biscuits-with-honey.png?resize=1024%2C1536" -o "images/blueberry-biscuits.png"

echo "Downloading: Applesauce Loaf"
curl -L --fail --silent --show-error "https://leitesculinaria.com/wp-content/uploads/2022/09/applesauce-bread.jpg" -o "images/applesauce-loaf.jpg"

echo "Downloading: Refrigerator Rolls"
curl -L --fail --silent --show-error "https://old-mill.com/wp-content/uploads/2015/08/Grandmas-Refrigerator-Rolls-old-mill.jpg" -o "images/refrigerator-rolls.jpg"

echo "Downloading: Sweet Roll Dough"
curl -L --fail --silent --show-error "https://img.sndimg.com/food/image/upload/f_auto%2Cc_thumb%2Cq_55%2Cw_iw/v1/img/recipes/25/99/42/3Oa8kS06Te66XywisgDP_IMG_20150120_155342.jpg" -o "images/sweet-roll-dough.jpg"

echo "Downloading: Aitch Jay’s Fast Crustless Quiche"
curl -L --fail --silent --show-error "https://www.wellplated.com/wp-content/uploads/2020/11/Easy-Healthy-Crustless-Quiche-Recipe-Bacon.jpg" -o "images/aitch-jay-s-fast-crustless-quiche.jpg"

echo "Downloading: Pretzels (Olde Tyme / Lye)"
curl -L --fail --silent --show-error "https://www.kingarthurbaking.com/sites/default/files/styles/featured_image/public/2020-06/german-style-pretzels-.jpg?itok=MfXjWWv7" -o "images/pretzels-olde-tyme-lye.jpg"

echo "Downloading: Ia Cohen – Eierkuchen (Egg Pancakes)"
curl -L --fail --silent --show-error "https://germangirlinamerica.com/wp-content/uploads/2014/09/eierkuchen-goldgelb-3.jpg" -o "images/ia-cohen-eierkuchen-egg-pancakes.jpg"

echo "Downloading: Date Nut Bread"
curl -L --fail --silent --show-error "https://tastesbetterfromscratch.com/wp-content/uploads/2025/01/Date-Nut-Bread25-1.jpg" -o "images/date-nut-bread.jpg"

echo "Downloading: Grandma Moog’s Quick and Easy Cinnamon Rolls"
curl -L --fail --silent --show-error "https://img.sndimg.com/food/image/upload/f_auto%2Cc_thumb%2Cq_55%2Cw_iw/v1/img/recipes/36/76/55/picsQgnV1.jpg" -o "images/grandma-moog-s-quick-and-easy-cinnamon-rolls.jpg"
