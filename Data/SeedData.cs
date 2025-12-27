using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using Recipe_Webpage.Data;

namespace Recipe_Webpage;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        // Ensure database is created and migrations are applied
        await context.Database.MigrateAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create default admin user if it doesn't exist
        var adminEmail = "admin@recipes.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            // Default password - should be changed in production
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed sample recipes if none exist
        if (!await context.Recipes.AnyAsync())
        {
            context.Recipes.AddRange(
                new Recipe
                {
                    Title = "Rainbow Divinity",
                    Description = "A colorful candy treat with fluffy texture and nutty crunch, topped with tinted coconut. Yields about 4 1/2 dozen candies.",
                    Ingredients = "3 cups sugar\n3/4 cup light corn syrup\n3/4 cup hot water\n1/4 teaspoon salt\n2 egg whites\n1/2 (3-ounce) package Jell-O gelatin (any flavor)\n1 teaspoon vanilla\n1 cup chopped nuts\n3/4 cup flaked coconut, tinted (for sprinkling)",
                    Instructions = "1. Butter the sides of a heavy 2-quart saucepan. In the saucepan, combine sugar, corn syrup, hot water, and salt.\n2. Cook and stir until the sugar dissolves and the mixture comes to a boil.\n3. Continue cooking (without stirring) to the hard-ball stage (250°F). Remove from heat.\n4. Meanwhile, beat egg whites to soft peaks. Gradually add gelatin, beating to stiff peaks.\n5. Add vanilla. Slowly pour the hot syrup over the egg-white mixture, beating constantly on a mixer at high speed until soft peaks form and the mixture starts to lose its gloss.\n6. Stir in nuts.\n7. Drop by teaspoons onto waxed paper; sprinkle with tinted coconut.\n\nNote: For tinted coconut, put the flaked coconut in a zip-top bag with a few drops of food coloring; seal and shake until evenly colored.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 30,
                    Servings = 54,
                    Category = "Candy",
                    ImageUrl = "/images/recipes/Rainbow%20Divinity.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Vegetable Dip",
                    Description = "A quick and easy creamy dip perfect for vegetables. Family recipe from Mom.",
                    Ingredients = "1 packet Knorr Vegetable Soup Mix\n1 1/4 pints sour cream\n3 tablespoons Parmesan cheese (optional)",
                    Instructions = "1. Mix all ingredients together.\n2. Chill for 2 hours before serving.",
                    PrepTimeMinutes = 5,
                    CookTimeMinutes = 0,
                    Servings = 10,
                    Category = "Appetizers",
                    ImageUrl = "/images/recipes/Vegetable%20Dip.webp",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Barbecue Sauce",
                    Description = "A sweet and tangy homemade barbecue sauce. Family recipe from Mom.",
                    Ingredients = "2 cups ketchup\n1/2 cup water\n1/4 cup lemon juice\n1/4 teaspoon chili powder\n1 teaspoon celery seed\n1 teaspoon Worcestershire sauce\n1 onion, grated fine\n1 teaspoon salt\n2 cups brown sugar",
                    Instructions = "1. Combine all ingredients in a saucepan.\n2. Bring to a boil.\n3. Simmer about 20 minutes, or until it reaches the taste you want.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 20,
                    Servings = 16,
                    Category = "Sauces",
                    ImageUrl = "/images/recipes/Barbecue%20Sauce.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Buckeyes",
                    Description = "Classic peanut butter and chocolate candies that look like buckeye nuts. Yields about 166 candies.",
                    Ingredients = "2 pounds creamy peanut butter\n1 pound butter, room temperature\n3 pounds confectioners' sugar\n2 packages (12 ounces each) chocolate chips\n1/2 bar paraffin",
                    Instructions = "1. Mix peanut butter, room-temperature butter, and confectioners' sugar until smooth. Refrigerate.\n2. Form small balls. Refrigerate.\n3. Melt chocolate chips and paraffin together.\n4. Dip balls into chocolate. Let dry on waxed paper.",
                    PrepTimeMinutes = 60,
                    CookTimeMinutes = 15,
                    Servings = 166,
                    Category = "Candy",
                    ImageUrl = "/images/recipes/Buckeyes.jpeg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Baked Eggs for Sandwiches",
                    Description = "Perfect baked eggs for sandwiches - no flipping required! Baked in a water bath for even cooking.",
                    Ingredients = "8 eggs\n1/2 teaspoon salt\n2/3 cup water\nVegetable oil spray (for pan)",
                    Instructions = "1. Heat oven to 300°F.\n2. Whisk eggs and salt. Whisk in 2/3 cup water.\n3. Spray an 8-inch square pan with vegetable oil spray. Pour egg mixture into pan.\n4. Set pan into a rimmed baking sheet. Add 1 1/2 cups water to the baking sheet.\n5. Bake 35–40 minutes, or until set.\n6. Let cool 10 minutes. Cut into 4 equal pieces for use in sandwiches.",
                    PrepTimeMinutes = 5,
                    CookTimeMinutes = 40,
                    Servings = 4,
                    Category = "Breakfast",
                    ImageUrl = "/images/recipes/Baked%20Eggs%20for%20Sandwiches.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Oatmeal Dinner Rolls",
                    Description = "Soft and hearty oatmeal dinner rolls with whole-wheat flour and molasses. Perfect for dinner or to freeze for later.",
                    Ingredients = "3/4 cup old-fashioned rolled oats, plus 4 teaspoons for sprinkling\n2/3 cup boiling water, plus 1/2 cup cold water\n2 tablespoons unsalted butter, cut into 4 pieces\n1 1/2 cups bread flour\n3/4 cup whole-wheat flour\n1/4 cup molasses\n1 1/2 teaspoons instant or rapid-rise yeast\n1 teaspoon table salt\n1 large egg, beaten with 1 teaspoon water and pinch table salt (egg wash)",
                    Instructions = "1. Stir 3/4 cup oats, boiling water, and butter together in bowl of stand mixer and let sit until butter is melted and most of water has been absorbed, about 10 minutes.\n2. Add bread flour, whole-wheat flour, cold water, molasses, yeast, and salt. Fit mixer with dough hook and mix on low speed until flour is moistened, about 1 minute. Increase speed to medium-low and mix until dough clears sides of bowl, about 8 minutes, scraping down dough hook halfway through mixing.\n3. Transfer dough to counter, shape into ball, and transfer to lightly greased bowl. Cover with plastic wrap and let rise until doubled in volume, 1 to 1 1/4 hours.\n4. Grease a 9-inch round cake pan. Transfer dough to lightly floured counter. Pat dough gently into an 8-inch square of even thickness. Cut dough into 12 pieces (3 rows by 4 rows). Form pieces into smooth, taut balls.\n5. Arrange seam side down in prepared pan, placing 9 dough balls around edge of pan and remaining 3 in center. Cover with plastic and let rise until doubled in size and no gaps are visible between them, 45 minutes to 1 hour.\n6. Adjust oven rack to lower-middle position and heat oven to 375°F. Brush rolls with egg wash and sprinkle with remaining 4 teaspoons oats.\n7. Bake until rolls are deep brown and register at least 195°F at center, 25 to 30 minutes.\n8. Let rolls cool in pan on wire rack for 3 minutes; invert rolls onto rack, then reinvert. Let cool at least 20 minutes before serving.\n\nNotes: Use blackstrap molasses sparingly as it can be bitter. Rolls freeze well; thaw at room temperature and refresh in a 350°F oven for about 8 minutes.",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 30,
                    Servings = 12,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/Oatmeal%20Dinner%20Rolls.webp",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Pumpkin Bread",
                    Description = "Classic moist pumpkin bread with warm spices, nuts, and raisins. Makes 2 loaves.",
                    Ingredients = "2/3 cup shortening\n2 2/3 cups sugar\n4 eggs\n1 1/3 cups pumpkin puree\n1 2/3 cups water\n3 1/3 cups flour\n2 teaspoons baking soda\n1/2 teaspoon salt\n1 1/2 teaspoons baking powder\n1 teaspoon cinnamon\n1 teaspoon cloves\n2/3 cup nuts\n2/3 cup raisins",
                    Instructions = "1. Cream shortening and sugar. Add eggs.\n2. Add pumpkin and water.\n3. Blend in dry ingredients (flour, soda, salt, baking powder, cinnamon, cloves).\n4. Stir in nuts and raisins.\n5. Pour into 2 greased 9 x 5 x 3-inch loaf pans.\n6. Bake at 350°F for 70 minutes.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 70,
                    Servings = 24,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/Pumpkin%20Bread.webp",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "P.B. Egg Bread",
                    Description = "A simple homemade yeast bread from Sue Sullivan's kitchen. Perfect for sandwiches or toast.",
                    Ingredients = "1/4 cup warm water\n1 package yeast\n1/2 cup buttermilk\n1 tablespoon sugar\n1 teaspoon salt\n1 egg\n1 tablespoon soft butter\n2 3/4 to 3 cups flour",
                    Instructions = "1. Dissolve yeast in warm water. Add with 1/2 cup flour; mix thoroughly.\n2. Add remaining ingredients and enough flour to make dough you can handle.\n3. Knead until smooth. Let rise until doubled, about 1 1/2 hours.\n4. Shape into a loaf. Cover with a damp towel and let rise until doubled again.\n5. Bake at 425°F for 25–30 minutes.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 30,
                    Servings = 12,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/PB%20Egg%20Bread.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Granola",
                    Description = "Homemade granola with oats, coconut, pumpkin seeds, and almonds sweetened with maple syrup. From Karen's kitchen. About 109 calories per 1/4 cup.",
                    Ingredients = "1/4 cup coconut oil, melted\n3/4 cup maple syrup\n3 cups old-fashioned oats\n1 cup unsweetened coconut\n3/4 cup raw pumpkin seeds\n3/4 cup sliced almonds or sunflower seeds\n1 teaspoon salt",
                    Instructions = "1. Heat oven to 300°F.\n2. Mix melted coconut oil and maple syrup.\n3. In a large bowl, mix oats, coconut, pumpkin seeds, almonds (or sunflower seeds), and salt.\n4. Stir liquid mixture into dry mixture, coating evenly.\n5. Spread in a jelly roll pan.\n6. Bake, stirring every 15 minutes, until done (45–55 minutes).",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 55,
                    Servings = 28,
                    Category = "Breakfast",
                    ImageUrl = "/images/recipes/Granola.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Blue Blueberry Muffins",
                    Description = "Extra blueberry-filled muffins using both fresh blueberries and blueberry pie filling. Recipe from Mom. Can also be made as a loaf.",
                    Ingredients = "3/4 cup melted butter (1 1/2 sticks)\n1 cup sugar\n2 eggs, beaten\n2 teaspoons baking powder\n1/4 teaspoon salt\n1 cup fresh or frozen blueberries (do not thaw if frozen)\n1/2 cup blueberry pie filling\n2 cups plus 1 tablespoon flour (reserve 1 tablespoon to coat berries)\n1/2 cup milk",
                    Instructions = "1. Grease the bottoms only of a 12-cup muffin pan (or use cupcake papers). Melt the butter. Mix in the sugar. Then add beaten eggs, baking powder, and salt; mix thoroughly.\n2. Put 1 tablespoon flour in a plastic bag, add the blueberries, and shake gently to coat. Set aside.\n3. Add half the remaining flour and half the milk to the bowl with the sugar mixture and mix. Add the remaining flour and milk and mix thoroughly.\n4. Add the blueberry pie filling and mix in. (Dough will turn blue but will fade when baked.)\n5. Fold in the flour-coated blueberries.\n6. Fill muffin tins 3/4 full. Bake 25–30 minutes at 350°F if pans are dark colored, or 375°F if pans are light colored.\n7. Remove from oven and let set 30 minutes to cool before removing from pan (helps prevent breaking).\n\nNote: Can also be baked as a loaf of blueberry bread instead of muffins; bake about 10 minutes longer.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 30,
                    Servings = 12,
                    Category = "Breakfast",
                    ImageUrl = "/images/recipes/Blueberry%20Muffins.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Kathy's Breakfast Casserole",
                    Description = "An overnight breakfast casserole with cinnamon bread. Perfect for holiday mornings - prep the night before and bake in the morning.",
                    Ingredients = "1 loaf cinnamon bread, cubed (Pepperidge Farm recommended)\n3 cups milk\n3 eggs\n3/4 cup Egg Beaters\n2 teaspoons vanilla",
                    Instructions = "1. Put bread cubes in a sprayed 9 × 13-inch Pyrex dish.\n2. Mix milk, eggs, Egg Beaters, and vanilla.\n3. Pour mixture over bread.\n4. Refrigerate overnight.\n5. Bake at 350°F for 50 minutes.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 50,
                    Servings = 8,
                    Category = "Breakfast",
                    ImageUrl = "/images/recipes/Kathy's%20Breakfast%20Casserole.jpeg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Blueberry Biscuits",
                    Description = "Tender blueberry biscuits with honey butter from America's Test Kitchen. Fresh blueberries preferred but frozen work too.",
                    Ingredients = "Biscuits:\n11 tablespoons unsalted butter (1 tablespoon melted, 10 tablespoons cut into 1/2-inch pieces and chilled)\n3 cups (15 ounces) all-purpose flour\n1/2 cup (3 1/2 ounces) sugar\n2 teaspoons baking powder\n1/2 teaspoon baking soda\n1 1/4 teaspoons table salt\n7 1/2 ounces (1 1/2 cups) blueberries\n1 1/3 cups buttermilk, chilled\n\nHoney Butter:\n2 tablespoons unsalted butter\n1 tablespoon honey\nPinch table salt",
                    Instructions = "1. Adjust oven rack to middle position and heat oven to 400°F. Brush bottom and sides of an 8-inch square baking pan with melted butter.\n2. Whisk flour, sugar, baking powder, baking soda, and salt together in a large bowl. Add chilled butter to flour mixture and smash butter between your fingertips into flat, irregular pieces. Add blueberries and toss with flour mixture. Gently stir in buttermilk until no dry pockets of flour remain.\n3. Using rubber spatula, transfer dough to prepared pan and spread into even layer and into corners of pan. Using bench scraper sprayed with vegetable oil spray, cut dough into 9 equal squares (2 cuts by 2 cuts), but do not separate. Bake until browned on top and paring knife inserted into center biscuit comes out clean, 40 to 45 minutes.\n4. Meanwhile, combine butter, honey, and salt in a small bowl and microwave until butter is melted, about 30 seconds. Stir to combine; set aside.\n5. Remove pan from oven and let biscuits cool in pan for 5 minutes. Turn biscuits out onto baking sheet, then reinvert biscuits onto wire rack. Brush tops of biscuits with honey butter (use all of it). Let cool for 10 minutes. Using serrated knife, cut biscuits along scored marks and serve warm.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 45,
                    Servings = 9,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/Blueberry%20Biscuits.jpeg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Applesauce Loaf",
                    Description = "A moist quick bread with applesauce, warm spices, and a sweet sugar glaze. Family recipe from Mom.",
                    Ingredients = "1/2 cup shortening\n1 cup sugar\n2 eggs\n1 3/4 cups sifted flour\n1 teaspoon salt\n1 teaspoon baking powder\n1/2 teaspoon baking soda\n1/2 teaspoon cinnamon\n1/2 teaspoon nutmeg\n1 cup applesauce\n1/2 cup chopped nuts\n\nSugar Glaze:\n1/2 cup sifted confectioners' sugar\n1 tablespoon water",
                    Instructions = "1. Stir shortening to soften; gradually add sugar, creaming until light.\n2. Add eggs; beat till light and fluffy.\n3. Sift together dry ingredients; add to creamed mixture alternately with applesauce.\n4. Stir in nuts.\n5. Pour into a paper-lined 9 1/2 × 5 × 3-inch loaf pan.\n6. Bake at 350°F for 1 hour.\n7. Cool in pan 10 minutes before removing.\n8. While warm, spread with sugar glaze (combine confectioners' sugar and water).",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 60,
                    Servings = 12,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/applesauce-loaf.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Refrigerator Rolls",
                    Description = "Versatile yeast rolls that can be refrigerated and baked as needed. Makes 4 dozen rolls. Family recipe from Mom.",
                    Ingredients = "2 packages yeast\n2 cups lukewarm water\n1/2 cup sugar\n1 teaspoon salt\n6 1/2 to 7 cups sifted flour\n1 egg\n1/4 cup shortening",
                    Instructions = "1. Dissolve yeast in lukewarm water.\n2. Add sugar, salt, and about half of the flour. Beat thoroughly for 2 minutes.\n3. Add egg and shortening, blending well.\n4. Gradually add remaining flour and mix until smooth.\n5. Cover with a damp cloth, then overwrap with plastic. Place in refrigerator.\n6. Punch down occasionally as dough rises in refrigerator.\n7. About 2 hours before baking, cut off amount needed; return balance to refrigerator.\n8. Shape chilled dough into rolls and place on greased baking sheets. Cover and let rise until light (1–2 hours).\n9. Bake 12–15 minutes at 400°F.\n10. For bread sticks: brush with egg white and sprinkle with sesame or poppy seed.",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 15,
                    Servings = 48,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/Refrigerator%20Rolls.jpeg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Sweet Roll Dough",
                    Description = "Basic sweet yeast dough for 60 rolls. Can be shaped into Parker House rolls, cinnamon rolls, and more. Family recipe from Mom.",
                    Ingredients = "2 cups milk, scalded\n2 tablespoons sugar\n4 tablespoons butter\n1/2 teaspoon salt\n1 package yeast\n2 eggs\n5 1/4 cups sifted flour",
                    Instructions = "1. Scald milk. Add sugar, butter, and salt; stir to dissolve. Cool to lukewarm.\n2. Add yeast; stir to dissolve.\n3. Add eggs; beat in.\n4. Stir in part of the flour; knead in the rest. Use just enough flour so dough can be handled easily.\n5. Place dough in bowl. Brush top with melted butter. Cover and let rise to double in bulk in a warm place (about 2 hours).\n6. Roll and shape as desired (Parker House, cinnamon, etc.). Put in greased pan and let rise until light (about 35 minutes).\n7. Bake at 425°F for about 20 minutes.",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 20,
                    Servings = 60,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/sweet-roll-dough.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Aitch Jay's Fast Crustless Quiche",
                    Description = "A quick crustless quiche perfect for Sunday brunch or a fast supper. The blender does all the work!",
                    Ingredients = "Butter, for greasing a 10-inch pie plate\n4 ounces cheese, shredded (Swiss, Cheddar, etc.)\n6–7 slices crisp bacon, crumbled (or chopped cooked ham)\nA few sautéed mushrooms (optional)\n4 eggs, unbeaten\n1/2 cup chopped onion\n1/2 cup flour\n2 tablespoons butter or margarine\n1/2 teaspoon salt, plus pepper to taste\n1 1/2 cups nonfat milk",
                    Instructions = "1. Butter a 10-inch pie plate or a regular-size quiche pan.\n2. Sprinkle in cheese, bacon (or ham), and mushrooms (if using).\n3. In a blender, combine eggs, onion, flour, butter or margarine, salt, pepper, and milk.\n4. Blend for 1 minute (or beat hard with an eggbeater if you don't have a blender).\n5. Pour over fillings in the pie plate and bake for 35 minutes at 350°F if the pan is metal; bake a bit longer if it's glass.\n6. Let stand 3–4 minutes before serving.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 35,
                    Servings = 6,
                    Category = "Breakfast",
                    ImageUrl = "/images/recipes/crustless-quiche.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Old-Fashioned Pretzels",
                    Description = "Traditional homemade soft pretzels using the old lye-dipping method. Makes 6 dozen. Use caution handling lye water.",
                    Ingredients = "1 cake yeast\n1 cup warm water\n2 tablespoons sugar\n2 quarts whole milk, warmed\n6–8 cups flour (first addition)\n1 cup lard, melted and totally cooled\nA handful of kosher salt\nMore flour, 6–8 cups additional (for a total of about 12–16 cups)\n\nFor dipping:\nLye water: 1 tablespoon Red Devil 100% lye + 1 quart water (boiling)\n\nFor topping:\nPretzel salt or kosher salt",
                    Instructions = "1. Dissolve yeast in warm water.\n2. Add sugar.\n3. Add warmed whole milk.\n4. Add 6–8 cups flour. (Mixture will be gooey.) Cover and let rise until double, about 1 hour.\n5. Add lard (melted and totally cooled) and a handful of kosher salt.\n6. Add flour and knead until soft and elastic, using enough flour to make workable dough (total flour about 12–16 cups).\n7. Shape into pretzel form: use a piece of dough about golf-ball size and roll as thin as a pencil.\n8. Let rise on a cloth for 1 hour, until doubled and bottom is dry.\n9. Dip in boiling lye water (use an old granite or porcelain bowl).\n10. Place on WELL-greased pans (use old pans—lye will leave marks). Sprinkle lightly with pretzel salt or kosher salt.\n11. Bake at 425°F for 5–6 minutes.",
                    PrepTimeMinutes = 60,
                    CookTimeMinutes = 6,
                    Servings = 72,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/pretzels.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Ia Cohen Egg Pancakes",
                    Description = "Traditional thin German-style egg pancakes (Eierkuchen) that are thin enough to roll. Serves 4.",
                    Ingredients = "5 eggs\n3 cups milk\n1/2 teaspoon salt\n2 3/4 cups flour\nButter or oil, for the pan (as needed)",
                    Instructions = "1. Beat eggs in a large bowl until combined.\n2. Whisk in milk and salt.\n3. Gradually whisk in flour until smooth (a few small lumps are okay).\n4. Optional (recommended): let batter rest 5–30 minutes.\n5. Heat a lightly buttered skillet or griddle over medium heat.\n6. Pour in enough batter to coat the pan in a thin layer, swirling the pan to spread it out.\n7. Cook until the top looks set and the underside is lightly golden, then flip and cook the other side briefly until golden.\n8. Repeat with remaining batter, adding a little more butter/oil as needed. Pancakes should be thin enough to roll.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 20,
                    Servings = 4,
                    Category = "Breakfast",
                    ImageUrl = "/images/recipes/egg-pancakes.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Date Nut Bread",
                    Description = "A moist, dense quick bread studded with dates and nuts. Makes 2 loaves. Family recipe from Mom.",
                    Ingredients = "2 cups water\n2 teaspoons baking soda\n1 cup chopped dates\n1 cup nuts\n1/2 cup shortening\n1 cup brown sugar\n1 cup white sugar\n2 eggs\n1 teaspoon vanilla\n4 cups flour",
                    Instructions = "1. Cook dates and baking soda in boiling water, stirring constantly, about 4 minutes.\n2. Add nuts and cool.\n3. Cream shortening and sugars.\n4. Add eggs and vanilla; beat till fluffy.\n5. Add flour and the date mixture; mix well.\n6. Bake in greased and floured pans at 325°F for 1 1/4–1 1/2 hours.\n7. Turn out onto racks immediately. Wrap and keep in refrigerator.",
                    PrepTimeMinutes = 25,
                    CookTimeMinutes = 90,
                    Servings = 24,
                    Category = "Breads",
                    ImageUrl = "/images/recipes/Date%20Nut%20Bread.jpeg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Grandma Moog's Quick Cinnamon Rolls",
                    Description = "Quick and easy cinnamon rolls that don't require yeast - ready in under an hour! Makes 18 rolls. From Penzeys.",
                    Ingredients = "Dough:\n3 cups flour\n1/4 teaspoon salt\n4 teaspoons baking powder\n1/2 cup sugar\n2 eggs\n2/3 cup milk\n1/2 teaspoon pure vanilla extract\n1/2 cup shortening, melted (and cooled slightly)\n\nFilling:\n1/2 cup white sugar\n1/4 cup melted butter\n\nTopping:\n1 1/2 teaspoons cinnamon\n1/2 cup brown sugar\n1/2 cup broken walnuts\n3 tablespoons melted butter\n\nIcing:\n1 cup powdered sugar\n2 tablespoons warm milk\n1 tablespoon melted butter\n1/4 teaspoon pure vanilla extract",
                    Instructions = "1. Sift flour with salt, baking powder, and sugar into a large mixing bowl and make a well in the center.\n2. In another bowl, beat eggs until well mixed. Pour into the center of the dry ingredients along with milk, vanilla, and melted shortening (warm, not hot).\n3. Mix by hand with a wooden spoon, or on the lowest speed with a mixer.\n4. Once dough is holding together, turn out onto a floured board and knead for 1 minute. If sticky, sprinkle a bit more flour as you work.\n5. Roll dough into a rectangle about 1/4-inch thick (about 9 × 13 inches).\n6. Combine filling ingredients; spread over dough, smoothing from the middle to within 1/2 inch of the edge.\n7. Roll dough up jellyroll-style. Using a sharp knife, cut evenly into 18 pieces.\n8. Mix topping ingredients. Grease an 18-cup muffin pan and divide topping between muffin tins.\n9. Place one piece of dough, cut side down, in each muffin tin on top of the topping.\n10. Bake at 350°F for 20–25 minutes, until golden brown.\n11. Remove rolls right away to cool. If topping stays in the tin, spoon it out and replace it on top of the roll while still hot.\n12. Let cool, then ice if desired (mix icing ingredients). Serve upside down or right side up.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 25,
                    Servings = 18,
                    Category = "Breakfast",
                    ImageUrl = "/images/recipes/cinnamon-rolls.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Overnight French Toast",
                    Description = "A make-ahead French toast casserole from EatingWell magazine (Nov/Dec 2017). Perfect for holiday mornings - prep the night before and bake in the morning. Serves 16.",
                    Ingredients = "Cooking spray\n1 pound whole-grain bread, diced into 1-inch pieces\n3 cups fresh or frozen fruit (mango, apples, pears, or berries) OR 1 cup dried fruit (cherries, cranberries, currants, or raisins)\n8 large eggs\n3 cups reduced-fat milk (decrease by 1/2 cup for denser texture)\n1/2 cup (1 stick) unsalted butter, melted\n3 tablespoons packed light brown sugar\n2 teaspoons vanilla extract\n1 tablespoon spice (ground cardamom, cinnamon, ginger, nutmeg, or pumpkin pie spice)\n1/4 teaspoon salt\n3/4 cup topping (chopped nuts, sliced nuts, or unsweetened coconut)\n2 cups ham, cut up (optional)",
                    Instructions = "1. Coat a 9-by-13-inch baking dish with cooking spray. Toss bread and fruit in the pan.\n2. In a large bowl, whisk eggs, milk, melted butter, brown sugar, vanilla, spice, and salt.\n3. Pour custard over bread mixture. Sprinkle with topping. Coat a piece of foil with cooking spray and cover the pan, coated-side down. Place another 9-by-13-inch baking dish on top as a weight. Refrigerate at least 8 hours (or up to 1 day).\n4. Let stand at room temperature while the oven preheats. Heat oven to 350°F. Remove the weight. Bake covered for 45 minutes. Uncover and bake 20 to 25 minutes more. Let stand 10 minutes before serving.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 70,
                    Servings = 16,
                    Category = "Breakfast",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Cherry Kuchen",
                    Description = "A German-style coffee cake made with frozen bread dough and cherry pie filling. Easy and delicious!",
                    Ingredients = "Cooking spray\n1 loaf frozen white bread dough, thawed per package directions\n1 1/4 to 1 1/2 cans cherry pie filling\n1/2 cup flour\n1/4 cup sugar\n2 tablespoons margarine",
                    Instructions = "1. Heat oven to 375°F. Spray a 9-by-13-inch pan.\n2. Let thawed dough sit at room temperature for 30 minutes.\n3. With floured hands, pat dough to cover the bottom of the 9-by-13-inch pan, bringing it up the sides a bit.\n4. Spread pie filling over the dough.\n5. Combine sugar, flour, and margarine; sprinkle over the cherries.\n6. Cover and let rise at room temperature for 45 to 60 minutes.\n7. Remove cover and bake 30 to 35 minutes.\n\nNotes: If the dough gets too warm, it gets too sticky. Can also use apple pie filling.",
                    PrepTimeMinutes = 45,
                    CookTimeMinutes = 35,
                    Servings = 12,
                    Category = "Desserts",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Hash Brown, Egg & Ham Skillet",
                    Description = "A quick and hearty one-pan breakfast with hash browns, eggs, and ham. Perfect for busy mornings.",
                    Ingredients = "Frozen hash brown potatoes\nNon-stick cooking spray\n4 eggs (or Egg Beaters)\nSalt\nPepper\nOnion powder\n2 or 3 slices ham, cut up",
                    Instructions = "1. Spray a non-stick skillet. Brown the frozen hash brown potatoes in the skillet.\n2. Put eggs (or Egg Beaters) on top of the browned potatoes.\n3. Scramble with salt, pepper, onion powder, and ham, stirring often, until eggs are cooked and ham is browned evenly.",
                    PrepTimeMinutes = 5,
                    CookTimeMinutes = 15,
                    Servings = 2,
                    Category = "Breakfast",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Zucchini Muffins",
                    Description = "Moist and flavorful muffins featuring fresh zucchini with warm cinnamon and nutmeg. Makes 12 muffins.",
                    Ingredients = "1 egg\n1/2 cup oil\n2 teaspoons vanilla\n1 cup sugar\n1 cup zucchini, grated\n1/2 teaspoon baking soda\n1 teaspoon cinnamon\n1 teaspoon salt\n2 teaspoons nutmeg\n2 cups flour",
                    Instructions = "1. Mix egg, oil, vanilla, and sugar.\n2. Add dry ingredients; mix.\n3. Stir in zucchini.\n4. Bake in 12 cupcake tins at 350°F for about 30 minutes.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 30,
                    Servings = 12,
                    Category = "Breakfast",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Pull-Apart Rolls",
                    Description = "Sweet and sticky pull-apart rolls made with frozen bread dough and butterscotch pudding mix. Easy and irresistible!",
                    Ingredients = "1 loaf frozen bread dough\n3 tablespoons margarine, melted\n4 heaping teaspoons brown sugar\nAbout 1/3 package butterscotch pudding mix (regular, not instant)",
                    Instructions = "1. Keep dough covered with wax paper while thawing. When thawed, cut loaf in half lengthwise; cut each half into 8 pieces (16 pieces total).\n2. Place pieces crosswise in 2 rows in a 5-by-9-inch loaf pan (they will appear crowded).\n3. Spoon melted margarine between the pieces.\n4. Sprinkle brown sugar and butterscotch pudding mix over pieces.\n5. Let rise until almost double.\n6. Bake at 350°F for 35 to 40 minutes. Put a cookie sheet under pan in oven in case it bakes over.\n7. Let stand 10 minutes, then turn out while very warm.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 40,
                    Servings = 16,
                    Category = "Breads",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Agg Kaka (Egg Cake)",
                    Description = "A traditional Swedish egg pancake (Äggkaka) from Shirley Markus. Puffs up beautifully in the oven. Serve with powdered sugar, syrup, and berries.",
                    Ingredients = "3 eggs, beaten\n2 cups milk\n1 cup flour\n1 teaspoon sugar\n1 teaspoon salt\n2 tablespoons butter",
                    Instructions = "1. Put butter in a 9-by-9-inch pan; place in oven to melt (425°F).\n2. Beat eggs; stir in milk. Add flour, sugar, and salt; mix.\n3. Carefully pour into hot pan with melted butter.\n4. Bake at 425°F for 20 to 25 minutes, until risen and browned.\n\nServing suggestion: Top with powdered sugar and/or syrup and berries.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 25,
                    Servings = 4,
                    Category = "Breakfast",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Buttermilk Pancakes",
                    Description = "Classic fluffy buttermilk pancakes. A simple, traditional recipe.",
                    Ingredients = "2 eggs\n2 cups sour milk or buttermilk\n2 1/2 cups flour\n1 teaspoon baking soda\n2 teaspoons baking powder\n1 teaspoon salt\n2 teaspoons sugar\n2 tablespoons melted butter",
                    Instructions = "1. Beat eggs in a large bowl.\n2. Add buttermilk and melted butter; mix well.\n3. In a separate bowl, whisk together flour, baking soda, baking powder, salt, and sugar.\n4. Add dry ingredients to wet ingredients and stir until just combined (some lumps are okay).\n5. Heat a griddle or skillet over medium heat and lightly grease.\n6. Pour about 1/4 cup batter per pancake onto the griddle.\n7. Cook until bubbles form on the surface and edges look set, then flip and cook until golden brown.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 15,
                    Servings = 6,
                    Category = "Breakfast",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Oat & Cranberry Scone Mix",
                    Description = "A make-ahead scone mix with toasted oats and cranberries. Makes about 3 3/4 cups mix (enough for 2 batches of 6 scones each).",
                    Ingredients = "Mix:\n1 cup quick-cooking oats, uncooked\n2 1/2 cups all-purpose flour\n1/2 cup packed brown sugar\n2 tablespoons baking powder\n1/2 teaspoon baking soda\n1/2 teaspoon salt\n1 cup dried cranberries\n\nPer batch of scones:\n1 large egg\n5 tablespoons melted butter\n1/3 cup milk\n1/2 teaspoon vanilla extract",
                    Instructions = "1. Preheat oven to 400°F. Spread oats in a 13-by-9-inch baking pan. Bake 8 to 10 minutes, stirring occasionally, until toasted; cool in pan on a wire rack.\n2. In a large bowl, combine flour, brown sugar, baking powder, baking soda, and salt. Stir to mix well, breaking up any lumps of brown sugar with your fingers. Stir in oats and cranberries.\n3. Divide mix in half. Store each half in a tightly covered container at room temperature up to 1 month.\n\nTo prepare scones (per batch): In a medium bowl, beat 1 large egg with a fork. Remove 1 tablespoon beaten egg and reserve to brush on scones. Beat remaining egg with 5 tablespoons melted butter, 1/3 cup milk, and 1/2 teaspoon vanilla extract. Pour over 1 half-batch of scone mix; stir just until moistened. Let dough stand 5 minutes. Scrape dough onto prepared cookie sheet; pat to a 6-inch round. With a floured knife, cut round into 6 wedges; do not separate wedges. Brush with reserved egg. Bake at 400°F for 16 to 18 minutes, until golden brown.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 18,
                    Servings = 12,
                    Category = "Breakfast",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Crockpot Oatmeal",
                    Description = "Wake up to warm, ready-to-eat oatmeal! This slow cooker oatmeal with apples and cinnamon cooks overnight.",
                    Ingredients = "2 apples, peeled, cored, and sliced\n1/3 cup brown sugar\n2 teaspoons cinnamon\n4 cups water\n2 cups old-fashioned oatmeal\n1 teaspoon salt",
                    Instructions = "1. Mix apples, brown sugar, and cinnamon; put in bottom of crockpot.\n2. Mix water, oats, and salt; pour on top.\n3. Cook 8 to 9 hours on Low. Stir thoroughly before serving.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 540,
                    Servings = 6,
                    Category = "Breakfast",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
