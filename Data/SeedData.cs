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

        // Ensure database is created (works for both SQLite and SQL Server)
        await context.Database.EnsureCreatedAsync();

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
                },
                // Recipes from digitized_recipes_final.pdf
                new Recipe
                {
                    Title = "Brown Sugar Frosting",
                    Description = "A simple caramel-style frosting made with brown sugar, cream, and butter. Family recipe from Mom.",
                    Ingredients = "4 tablespoons brown sugar\n4 tablespoons cream\n4 tablespoons butter\nPowdered sugar (to consistency)",
                    Instructions = "1. Combine brown sugar, cream, and butter; bring to a boil.\n2. Remove from heat and add powdered sugar until it reaches the proper consistency.",
                    PrepTimeMinutes = 5,
                    CookTimeMinutes = 5,
                    Servings = 8,
                    Category = "Frostings",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Seven Minute Icing",
                    Description = "A light, fluffy meringue-style icing made in a double boiler. Family recipe from Mom.",
                    Ingredients = "1 unbeaten egg white\n2/3 cup sugar\n3 tablespoons cold water\n1/4 teaspoon baking powder\n1 teaspoon vanilla",
                    Instructions = "1. Combine all ingredients in the top of a double boiler.\n2. Beat constantly for 7 minutes using an electric mixer.",
                    PrepTimeMinutes = 2,
                    CookTimeMinutes = 7,
                    Servings = 8,
                    Category = "Frostings",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Apple Cake",
                    Description = "A moist, spiced cake loaded with fresh apples. Bakes in a 9×13-inch pan. Family recipe from Mom.",
                    Ingredients = "2 cups sugar\n1 cup margarine\n2 eggs\n2 cups flour\n2 teaspoons baking soda\n2 teaspoons cinnamon\n2 teaspoons nutmeg\n2 teaspoons vanilla\n4 cups peeled tart apples",
                    Instructions = "1. Cream together sugar and margarine; add eggs.\n2. Sift together flour, baking soda, cinnamon, and nutmeg; add to mixture.\n3. Stir in vanilla.\n4. Add apples last; stir to combine.\n5. Bake in a greased, floured 9×13-inch pan at 350°F for 1 hour (or until apples are done).",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 60,
                    Servings = 12,
                    Category = "Desserts",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Buttercream Frosting",
                    Description = "Classic buttercream frosting. Enough for two 8-inch or 9-inch cake layers. Family recipe from Mom.",
                    Ingredients = "1 pound powdered sugar\n1/2 teaspoon salt\n1/4 cup milk\n1 teaspoon vanilla\n1/3 cup soft butter or margarine",
                    Instructions = "1. Combine all ingredients in a mixing bowl.\n2. Beat with an electric mixer until smooth.\n3. If too stiff, add a few drops of milk.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 0,
                    Servings = 16,
                    Category = "Frostings",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Grandma's Chocolate Cake",
                    Description = "A rich, moist chocolate cake. Bakes in a 9×13-inch pan or two round layers. Family recipe from Mom (labeled 'Grandma's').",
                    Ingredients = "Milk mixture:\n1 cup milk\n1 tablespoon vinegar\n\nCake batter:\n1 cup shortening\n2 cups sugar\n2 eggs\n1/2 cup cocoa\n2 1/2 cups flour\n2 teaspoons baking soda\n1/2 teaspoon salt\n1 cup hot water\n1 teaspoon vanilla",
                    Instructions = "1. Stir vinegar into milk and let set.\n2. Cream sugar and shortening.\n3. Add eggs, one at a time.\n4. Add cocoa.\n5. Combine flour, baking soda, and salt; add alternately with the milk mixture. Blend just until smooth.\n6. Stir in hot water and vanilla.\n7. Bake at 350°F for 40–45 minutes (in a greased & floured 9×13-inch pan, or in two prepared round pans).",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 45,
                    Servings = 12,
                    Category = "Desserts",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Grandma's Chocolate Cake Frosting",
                    Description = "A simple chocolate frosting to pair with Grandma's Chocolate Cake. Family recipe from Mom.",
                    Ingredients = "1/3 cup cocoa\n3 cups powdered sugar\n1/4 cup margarine\n1 teaspoon vanilla\nWarm water (to consistency)",
                    Instructions = "1. Combine cocoa, powdered sugar, margarine, and vanilla.\n2. Add warm water a little at a time until the frosting reaches spreading consistency.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 0,
                    Servings = 12,
                    Category = "Frostings",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Blitzen Cake (Germany)",
                    Description = "A German layered cake with meringue topping and lemon filling. Recipe from Kathy. Serves 10–12.",
                    Ingredients = "Cake:\n1/2 cup shortening\n1/2 cup sugar\n4 egg yolks\n5 tablespoons milk\n1 cup cake flour\n1 teaspoon baking powder\n1/2 teaspoon vanilla\n\nMeringue topping:\n4 egg whites\n1 cup powdered sugar\n\nNut topping:\n1/2 cup chopped nuts\n\nLemon filling:\nJuice and rind of 1 lemon\n1 cup sugar\n1 egg, beaten",
                    Instructions = "1. Cream shortening and sugar.\n2. Beat in egg yolks; stir in milk.\n3. Sift together cake flour and baking powder; add to batter. Stir in vanilla.\n4. Put batter in 2 layer cake pans which have been slightly greased.\n5. Beat egg whites until stiff; add powdered sugar and beat well. Spread on top of each layer.\n6. Sprinkle chopped nuts on top. Bake at 350°F until browned. Cool.\n7. Filling: Combine lemon juice/rind, sugar, and beaten egg; cook in a double boiler until thick. Cool.\n8. Spread filling between cake layers.",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 30,
                    Servings = 12,
                    Category = "Desserts",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Chocolate Butter-Cream Frosting",
                    Description = "A rich chocolate buttercream frosting. Enough for three 8-inch layers or two 9-inch layers. Family recipe from Mom.",
                    Ingredients = "2 2/3 cups sifted powdered sugar\n3/4 cup cocoa\n1/4 teaspoon salt\n1 egg\n1/4 cup soft margarine\n3 tablespoons hot water",
                    Instructions = "1. Blend with a spoon, then beat with a beater until smooth.\n2. Can be mixed in the conventional way.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 0,
                    Servings = 16,
                    Category = "Frostings",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Chocolate Cupcakes",
                    Description = "Simple chocolate cupcakes. Makes 18 cupcakes. Family recipe from Mom.",
                    Ingredients = "1 cup sugar\n1/2 cup cocoa\n1/2 cup milk\n1/2 cup butter\n1 egg\n1 1/2 cups cake flour\n1 teaspoon baking powder\n1 teaspoon baking soda\n1/2 teaspoon salt\n1/2 cup boiling water\n1 teaspoon vanilla",
                    Instructions = "1. Combine all ingredients in a bowl.\n2. Beat 3 minutes.\n3. Bake at 350°F for 30 minutes.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 30,
                    Servings = 18,
                    Category = "Desserts",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Chocolate Frosting",
                    Description = "A creamy chocolate frosting. Makes about 2 cups. From Grandma's Chocolate Cake card.",
                    Ingredients = "6 tablespoons margarine\n3/4 cup cocoa\n2 2/3 cups confectioners' sugar\n1/3 cup milk\n1 teaspoon vanilla",
                    Instructions = "1. Cream margarine.\n2. Add cocoa and confectioners' sugar alternately with milk, mixing until smooth.\n3. Stir in vanilla.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 0,
                    Servings = 16,
                    Category = "Frostings",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                // Recipes from digitized_recipes_updated (2).pdf
                new Recipe
                {
                    Title = "Herb Bread",
                    Description = "A fragrant herb bread with basil, thyme, oregano, and nutmeg. Makes 2 loaves. Recipe from Deb Broad.",
                    Ingredients = "2 packages yeast\n1/4 cup water\n1/4 cup sugar\n2 teaspoons salt\n1/4 cup shortening\n1 cup milk\n~2 cups flour\n2 eggs\n1/4 teaspoon basil\n1/2 teaspoon thyme\n1/2 teaspoon oregano\n1 teaspoon nutmeg\n~3 cups flour (to make soft dough)",
                    Instructions = "1. Soften yeast in water.\n2. Scald milk and add to the other ingredients in a large mixing bowl; cool to lukewarm.\n3. Add flour to make a thick batter; add yeast; beat, then add eggs and mix well.\n4. Crumble the spices and add to batter.\n5. Add remaining flour to make a soft dough. Knead until smooth.\n6. Let rise until doubled. Punch down and shape into 2 loaves.\n7. Let rise again until doubled. Bake at 375°F for about 30-35 minutes.",
                    PrepTimeMinutes = 180,
                    CookTimeMinutes = 35,
                    Servings = 16,
                    Category = "Breads",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Biscuit Dough",
                    Description = "Classic homemade biscuits. Makes 16 medium biscuits.",
                    Ingredients = "2 cups unbleached all-purpose flour\n1/2 teaspoon salt\n2 teaspoons baking powder\n1/2 teaspoon cream of tartar\n2 tablespoons sugar\n1/3 cup shortening\n1/2 cup milk",
                    Instructions = "1. Mix flour, salt, baking powder, cream of tartar, and sugar.\n2. Cut in shortening.\n3. Add milk all at once; mix quickly.\n4. Turn dough onto a lightly floured board and knead lightly.\n5. Roll to 1/2 inch; cut with biscuit cutter.\n6. Place on an ungreased cookie sheet.\n7. Bake in a hot oven at 450°F for 10–12 minutes.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 12,
                    Servings = 16,
                    Category = "Breads",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Easy Huevos Rancheros",
                    Description = "Quick Mexican-style eggs with black beans, cheese, and avocado. Makes 4 main-dish servings.",
                    Ingredients = "1 ripe medium avocado\n6 large eggs\n1/4 cup water\n1 tablespoon margarine or butter\n1 cup mild salsa\n1 can (15 to 19 ounces) black beans, rinsed and drained\n3/4 cup shredded Mexican cheese blend (3 ounces)\n8 corn tortillas, warmed",
                    Instructions = "1. In cup, with fork, mash avocado. In bowl, with wire whisk, mix eggs and water.\n2. In nonstick 10-inch skillet, melt margarine over medium heat. Add egg mixture and cook 1 to 1 1/2 minutes or until eggs just begin to set, stirring constantly with heat-safe spatula or wooden spoon.\n3. Spoon salsa over eggs; top with beans and cheese. Cover and cook 2 to 3 minutes longer or until cheese melts and beans are heated through, but do not stir.\n4. Serve egg mixture with tortillas and avocado.",
                    PrepTimeMinutes = 5,
                    CookTimeMinutes = 10,
                    Servings = 4,
                    Category = "Breakfast",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "White Bread",
                    Description = "Classic homemade white bread. Makes 4 loaves. Family recipe from Mom.",
                    Ingredients = "2 packages yeast\n1/2 cup very warm water\n1/3 cup shortening\n1/3 cup sugar\n2 tablespoons salt\n2 cups hot scalded milk\n1 1/2 cups cold water\n11 to 12 cups sifted flour",
                    Instructions = "1. Soften yeast in the warm water.\n2. Combine shortening, sugar, salt, and hot scalded milk; stir to melt shortening.\n3. Add cold water to the mixture to cool; then add yeast and mix well.\n4. Blend in 11 to 12 cups sifted flour to form a stiff dough.\n5. Knead on a floured board until smooth and satiny, 5–10 minutes (turn 1/4 turn each time; add flour as needed).\n6. Place in a greased bowl, turning dough to grease all sides lightly. Cover. Let rise in a warm place (85°–90°F) until light and doubled in size, about 2 hours.\n7. Punch down dough by plunging fist in center. Fold edges toward center; turn upside down in bowl and cover. Let rise 1/2 hour.\n8. Place dough on lightly floured board and divide into 4 parts. Mold into balls; allow to rest, closely covered with an inverted bowl, for 15 minutes.\n9. Shape into loaves. Place in greased 9 x 5 x 3-inch pans and cover. Let rise in a warm place until dough fills pans and tops of loaves are well above pan edges, about 1 1/2 hours.\n10. Bake in a moderate oven at 375°F for 45 to 50 minutes. Do not store until cold.\n\nTo shape loaves: Flatten and stretch to an 18 x 10-inch rectangle. Fold one side to center—press out air pockets; fold other side to overlap. Fold over both ends pressing out air; roll into loaf.",
                    PrepTimeMinutes = 240,
                    CookTimeMinutes = 50,
                    Servings = 32,
                    Category = "Breads",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Snickerdoodle Bars",
                    Description = "Cinnamon-topped bar cookies. Bakes in a 9x9-inch pan. Family recipe from Mom.",
                    Ingredients = "1 3/4 cups flour\n2 1/2 teaspoons baking powder\n1/2 cup sugar\n1/2 teaspoon salt\n1/2 cup shortening\n1 egg\n3/4 cup milk\n\nTopping:\n1/4 cup sugar\n1 tablespoon butter\n1/2 teaspoon cinnamon",
                    Instructions = "1. Sift together flour, baking powder, sugar, and salt.\n2. Cut in shortening.\n3. Beat egg and add to milk; quickly stir into dry ingredients.\n4. Spread dough in a 9 x 9-inch pan; sprinkle with topping.\n5. Bake 20–25 minutes at 400°F.\n\nTopping: Mix 1/4 cup sugar, 1 tablespoon butter, and 1/2 teaspoon cinnamon.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 25,
                    Servings = 16,
                    Category = "Desserts",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Lemon Poppy Seed Bread",
                    Description = "An easy lemon bread made with cake mix. Makes 2 loaves.",
                    Ingredients = "1 package lemon cake mix\n1 (3-ounce) package lemon instant pudding\n1/2 cup salad oil\n1 cup water\n4 eggs\n2 tablespoons poppy seeds\n\nGlaze:\n1/4 cup sugar\n6 tablespoons lemon juice\nPowdered sugar for sprinkling",
                    Instructions = "1. Mix cake mix, pudding, oil, water, eggs, and poppy seeds.\n2. Divide between 2 loaf pans.\n3. Bake at 350°F for 40 minutes.\n4. When done, poke holes in top and pour over 1/4 cup sugar mixed with 6 tablespoons lemon juice.\n5. Sprinkle with powdered sugar.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 40,
                    Servings = 16,
                    Category = "Breads",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Danish Pastry",
                    Description = "A flaky almond-flavored pastry. Family recipe from Mom.",
                    Ingredients = "3 cups flour\n1 cup butter\n1 teaspoon almond flavoring\n3 eggs\nWater (used in steps below)",
                    Instructions = "1. Cut 1/2 cup butter into 1 cup flour. Add 2 tablespoons water. Shape into a ball; divide in half. Pat into 2 strips (about 10–13 inches long). Place strips 3 inches apart on an ungreased baking sheet.\n2. Mix 1/2 cup butter and 1 cup water; bring to a boil. Remove from heat and add almond flavoring.\n3. Beat in the remaining flour. Add eggs one at a time.\n4. Spread evenly over each piece of pastry.\n5. Bake in a 350°F oven for 1 hour.",
                    PrepTimeMinutes = 60,
                    CookTimeMinutes = 60,
                    Servings = 12,
                    Category = "Desserts",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                // Recipes from digitized_recipes_updated (3).pdf
                new Recipe
                {
                    Title = "Fried Chicken and Potatoes in One Pan",
                    Description = "A classic one-pan meal with pan-fried chicken and potatoes, served with homemade gravy.",
                    Ingredients = "4 to 6 pieces chicken\nFlour (for rolling/dredging chicken)\nAbout 4 tablespoons shortening (for browning)\nSalt\nPaprika\n2 to 4 potatoes, peeled and halved\n\nGravy:\nMore shortening as needed\nAbout 4 tablespoons flour\nAbout 2 cups water\nSalt to taste",
                    Instructions = "1. Roll chicken pieces in flour.\n2. Brown one side in about 4 tablespoons shortening.\n3. Sprinkle the unbrowned (up-side) with salt and paprika.\n4. Turn and sprinkle the other side (browned side) with salt and paprika.\n5. Peel potatoes and cut in half. Lay potatoes around browned chicken.\n6. Sprinkle potatoes with salt and paprika. Cover.\n7. Cook over low heat 45 minutes to 60 minutes. Chicken should be tender.\n8. Remove chicken and potatoes from pan.\n9. You may need to add more shortening to make gravy. Use about 4 tablespoons flour and stir into pan drippings.\n10. Blend in water while off heat—about 2 cups.\n11. Return to heat and, using a blender, stir until it boils. Salt to taste.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 60,
                    Servings = 4,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Tuna and Macaroni",
                    Description = "A quick and easy comfort food with tuna and macaroni in a creamy mushroom sauce. Family recipe from Mom.",
                    Ingredients = "1 1/2 cups large elbow macaroni (or 1 1/2 cups noodles)\n2 teaspoons salt\n1 can cream of mushroom soup\n1 can water or milk (may need more water)\n1 (6 1/2-ounce) can tuna, drained",
                    Instructions = "1. Cook macaroni/noodles as long as package says. Drain.\n2. Combine soup and 1 can water or milk. Bring to a boil; stir often. (May need more water.)\n3. Add drained tuna to soup mixture, then add macaroni/noodles. Stir.\n4. Bake at 350°F for 1/2 hour.\n5. If in a hurry: simmer on stove 5 minutes and serve, but use less water.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 30,
                    Servings = 3,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Beef Dressing Casserole",
                    Description = "A hearty casserole with ground beef and bread dressing, topped with mushroom soup. Recipe from Ethel Ploth.",
                    Ingredients = "1 small onion\n1/4 cup minced celery leaves\n1/4 teaspoon salt\n1/4 teaspoon pepper\n1 teaspoon poultry seasoning\n12 slices bread\n2 cans mushroom soup\n1 cup milk\n2 pounds ground beef\n1 tablespoon prepared mustard\n1 teaspoon Worcestershire sauce",
                    Instructions = "1. Mix onion, salt, pepper, poultry seasoning, and bread. Blend in 1 can soup + 1 cup milk.\n2. Cover and store in refrigerator overnight.\n3. Mix meat, Worcestershire sauce, and mustard.\n4. Add bread mixture and mix thoroughly.\n5. Put into a 13×9 pan.\n6. Bake 1/2 hour. Remove and spread last can of soup; cook for 1/2 hour more.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 60,
                    Servings = 8,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Crock-Pot Pot Roast",
                    Description = "A tender slow cooker pot roast with red wine and herbs, served with homemade gravy.",
                    Ingredients = "1 onion\n2 cloves garlic\n4 pounds rump roast\n2 teaspoons salt\n1/4 teaspoon pepper\n1/2 teaspoon dried rosemary\n1/2 teaspoon dried thyme\n3/4 cup red wine\n\nGravy:\n3 tablespoons flour\n1/4 cup water\n2 cups cooking liquid (measured from crock pot)",
                    Instructions = "1. Chop the onion and the garlic.\n2. Put the onions and garlic in the crock pot and set the roast on top.\n3. Sprinkle with the salt, pepper, rosemary, and thyme. Pour in the wine.\n4. Cover and cook on the High setting until the meat is very tender, about 5 hours.\n5. Remove the roast to a serving plate. Measure 2 cups of the cooking liquid into a saucepan.\n6. Stir together the flour and water and stir it into the cooking liquid.\n7. Bring to a boil, stirring; reduce heat and simmer 10 minutes.\n8. Slice the roast and serve with the gravy.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 310,
                    Servings = 10,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Slow Cooker Fajitas",
                    Description = "Easy slow cooker fajitas with tender sirloin steak and colorful peppers. Serve with tortillas, salsa, cheese, and cilantro.",
                    Ingredients = "1 each medium green, red, and yellow pepper, cut into 1/2-inch strips\n1 onion, thinly sliced\n2 pounds boneless sirloin steak, cut into thin strips\n3/4 cup water\n2 tablespoons red wine vinegar\n1 tablespoon lime juice\n2 teaspoons cumin\n1 teaspoon chili powder\n1/2 teaspoon salt\n1/2 teaspoon garlic powder\n1/2 teaspoon pepper\n1/2 teaspoon cayenne\n\nTo serve:\nTortillas\nSalsa\nCheese\nCilantro",
                    Instructions = "1. Put peppers and onion in the bottom of the slow cooker. Place steak on top.\n2. Add water, red wine vinegar, lime juice, cumin, chili powder, salt, garlic powder, pepper, and cayenne.\n3. Cook 8–9 hours on Low.\n4. To serve: Place about 3/4 cup meat down center of tortillas. Top with 1 tablespoon salsa, 1 tablespoon cheese, and 1 teaspoon cilantro.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 540,
                    Servings = 6,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Shepherd's Pie",
                    Description = "A simple and comforting shepherd's pie with ground beef, vegetable soup, and mashed potatoes.",
                    Ingredients = "3/4 pound ground beef\nSalt\nOnion\n1 can vegetable soup\nMashed potatoes (to top casserole)",
                    Instructions = "1. Brown ground beef; drain grease.\n2. Season with salt and add onion.\n3. Add 1 can vegetable soup.\n4. Place mixture in a casserole dish and top with mashed potatoes.\n5. Bake 1/2 hour at 350°F.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 30,
                    Servings = 3,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Chop Suey Casserole",
                    Description = "A classic American-Chinese casserole with ground beef, rice, celery, and mushroom soup. Family recipe from Mom.",
                    Ingredients = "1 1/2 pounds ground beef\n3 medium onions, chopped\n2 cups diced celery\n1/2 cup uncooked rice\n3 1/2 cups boiling water\n2 cans cream of mushroom soup\n2 1/2 tablespoons soy sauce\n1/2 tablespoon brown sugar",
                    Instructions = "1. Brown beef and celery, and lastly onions.\n2. Mix rice, water, soup, and remaining ingredients.\n3. Add to beef mixture.\n4. Pour into a greased 2-quart casserole.\n5. Bake 1 1/2 hours at 375°F.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 90,
                    Servings = 10,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                // Recipes from digitized_recipes_v3.pdf
                new Recipe
                {
                    Title = "Slow Cooker Hungarian Goulash",
                    Description = "A rich and flavorful Hungarian goulash made in the slow cooker with sweet paprika and sour cream. From Cook's Country. Serve over egg noodles or spaetzle.",
                    Ingredients = "1 (4-pound) boneless beef chuck-eye roast, trimmed and cut into 1 1/2-inch pieces\nSalt and pepper\n1 (12-ounce) jar roasted red peppers, rinsed\n1/2 cup sweet paprika\n2 tablespoons tomato paste\n1 tablespoon distilled white vinegar\n2 tablespoons vegetable oil\n4 pounds onions, chopped (about 6 cups)\n4 carrots, peeled and cut into 1-inch chunks\n3 tablespoons all-purpose flour\n1 bay leaf\n5 tablespoons water\n1/2 cup sour cream\n2 tablespoons minced fresh parsley",
                    Instructions = "1. Pat beef dry and season with 1 teaspoon salt and pepper. Transfer to slow cooker.\n2. Process red peppers, paprika, tomato paste, and vinegar in a food processor until smooth, about 2 minutes; set aside.\n3. Heat oil in a Dutch oven over medium heat. Add onions, carrots, and 1 teaspoon salt; cook, covered, until onions soften, 8 to 10 minutes.\n4. Stir in flour, bay leaf, and red pepper mixture; cook until mixture begins to brown and stick to bottom of pot, about 2 minutes.\n5. Stir in water, scraping up any browned bits.\n6. Stir onion mixture into slow cooker until beef is evenly coated. Cover and cook until meat is tender: 6 to 7 hours on HIGH, or 7 to 8 hours on LOW.\n7. Turn off slow cooker; let stew settle for 5 minutes, then skim fat and discard bay leaf.\n8. Combine sour cream and 1/2 cup hot stew liquid in a bowl (to temper sour cream), then stir mixture into stew. Season with salt and pepper to taste. Serve, sprinkled with parsley.",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 420,
                    Servings = 6,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Turkeyette Casserole",
                    Description = "A creamy turkey and noodle casserole with cheddar cheese. Family recipe from Mom.",
                    Ingredients = "2 cups medium noodles\n2 cups cooked turkey or chicken\n1/4 cup minced pimento\n1 cup cream of mushroom soup\n1/2 can water\n3/4 teaspoon celery salt\n1/4 teaspoon pepper\n1/2 small onion, grated\n1 1/2 cups sharp cheddar cheese, divided (save 1/2 cup for topping)",
                    Instructions = "1. Heat oven to 350°F.\n2. Mix everything except 1/2 cup of the cheese.\n3. Put in a 1 1/2-quart casserole and cover.\n4. Bake 45 minutes (covered).\n5. Just before serving, add remaining 1/2 cup cheese; return to oven uncovered until cheese melts.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 45,
                    Servings = 4,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Turkey Chow Mein",
                    Description = "A quick and easy turkey chow mein. Serve over rice and/or chow mein noodles.",
                    Ingredients = "4 tablespoons margarine, melted\n2 cups chopped celery\n1 green pepper, cut up (optional)\n1/2 cup cut-up onion (optional)\nTurkey, diced or sliced (2-3 cups)\n1 can chicken rice soup\n1 can water\n1 teaspoon salt\n1/4 teaspoon pepper\nCornstarch slurry: 3 tablespoons cornstarch + a little water\nRice and/or chow mein noodles, for serving",
                    Instructions = "1. Brown the vegetables in melted margarine.\n2. Add turkey, chicken rice soup, and 1 can water; simmer until hot.\n3. Stir in cornstarch slurry; cook until thickened, stirring.\n4. Serve over rice and/or chow mein noodles.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 20,
                    Servings = 4,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Lasagna",
                    Description = "A classic homemade lasagna with layers of noodles, cottage cheese mixture, mozzarella, and meat sauce. Family recipe from Mom.",
                    Ingredients = "1/2 to 3/4 pound ground beef\n1 small can tomato sauce\n1 teaspoon salt\n4 lasagna noodles\n\nCottage cheese mixture:\n1 (12-ounce) carton cottage cheese\n1 egg, beaten\n1 teaspoon salt\n1/4 teaspoon pepper\n1 tablespoon parsley flakes\n1/4 cup grated Parmesan cheese\n1 (6-ounce) package thin-sliced mozzarella cheese",
                    Instructions = "1. Heat oven to 375°F. Grease a casserole dish.\n2. Brown ground beef; drain. Add tomato sauce (and salt).\n3. Cook noodles in salted water; drain and rinse in cold water.\n4. Mix cottage cheese mixture ingredients.\n5. Layer: noodles → cottage cheese mixture → mozzarella cheese → meat mixture. Repeat layers.\n6. Bake 30 minutes at 375°F.",
                    PrepTimeMinutes = 25,
                    CookTimeMinutes = 30,
                    Servings = 8,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Turkey and Dumplings",
                    Description = "Comfort food with tender turkey, carrots, and fluffy homemade dumplings.",
                    Ingredients = "2 cups cooked turkey, cubed\n2 3/4 cups carrots, cut up\n3 chicken bouillon cubes\nWater, to cover\nOnion, about 1 tablespoon\nMilk + flour to thicken (about 1/2 cup milk)\n\nDumplings:\n1/2 cup milk\n2 tablespoons oil\n1 cup flour\n2 teaspoons baking powder\n1/2 teaspoon salt",
                    Instructions = "1. Combine turkey, carrots, bouillon, water to cover, and onion. Boil until carrots are tender.\n2. Thicken broth with flour and milk.\n3. Mix dumplings (dry ingredients + oil + milk).\n4. Drop dumplings onto simmering turkey mixture. Turn heat down, cover, and cook 12–15 minutes.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 35,
                    Servings = 4,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Easy Smoked Sausage Skillet",
                    Description = "A quick 20-minute skillet meal with smoked sausage, vegetables, and rice topped with mozzarella cheese. From Hillshire Farm.",
                    Ingredients = "1 (14-ounce) package smoked sausage, sliced diagonally into 1/4-inch slices\n1/4 cup olive oil\n2 cloves garlic, crushed\n1 large red bell pepper, sliced thin\n1 small yellow onion, sliced thin\n1 (10-ounce) package frozen broccoli, thawed\n1/2 cup chicken broth or water\n1/2 cup tomato sauce\n2 cups instant rice\n1/2 cup shredded mozzarella cheese",
                    Instructions = "1. Heat olive oil and crushed garlic; stir in smoked sausage slices and cook until sausage is browned.\n2. Add pepper, onion, broccoli, chicken broth/water, and tomato sauce; simmer about 10 minutes until vegetables are tender and liquid is absorbed.\n3. In the meantime, cook rice according to package instructions. Stir rice into skillet; sprinkle with cheese and serve.",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 20,
                    Servings = 4,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Chicken-Macaroni Casserole",
                    Description = "A creamy baked casserole with chicken, macaroni, cheddar cheese, and mushrooms. Can also be made with tuna.",
                    Ingredients = "1 1/2 cups cut-up cooked chicken (or turkey)\n1 1/2 cups uncooked elbow macaroni\n1 cup shredded Cheddar cheese (about 4 ounces)\n1 can (4 ounces) mushroom stems and pieces, drained\n1/4 cup chopped pimiento\n1 can (10 1/2 ounces) condensed cream of chicken soup\n1 cup milk\n1/2 teaspoon salt\n1/2 teaspoon curry powder, if desired",
                    Instructions = "1. Heat oven to 350°F.\n2. Stir together all ingredients. Pour into an ungreased 1 1/2-quart casserole.\n3. Cover; bake 1 hour.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 60,
                    Servings = 6,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Turkey Hash",
                    Description = "A hearty skillet hash with leftover turkey, potatoes, and green peppers. Great for using up holiday leftovers.",
                    Ingredients = "4 to 5 potatoes, peeled and cut into small cubes\n1 large green pepper, cored and cut into small cubes (or strips)\n1 onion, finely chopped (or a sprinkling of onion powder)\n1 to 2 cups chopped leftover cooked turkey breast\n1 teaspoon dried thyme\n1 to 2 tablespoons Worcestershire sauce\n3 tablespoons chopped parsley (or a sprinkling of dried)\n1/2 cup milk\nSalt and pepper to taste",
                    Instructions = "1. Spray a large nonstick skillet with pan spray.\n2. Sauté potatoes, pepper, and onion over medium-high heat for about 10 minutes, until potatoes start to soften.\n3. Stir in turkey, thyme, Worcestershire sauce, parsley, salt, and pepper.\n4. Pour milk over. Reduce heat and cook until milk has been absorbed and a crust has formed on bottom.\n5. Turn hash and cook until the second side is brown.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 25,
                    Servings = 4,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Beef Stroganoff (Slow Cooker)",
                    Description = "A creamy slow cooker beef stroganoff with mushrooms and sour cream. Serve over egg noodles.",
                    Ingredients = "1 package (14 ounces) white mushrooms, cleaned and quartered\n1 1/2 pounds beef chuck steak, trimmed and cut into 1-inch cubes\n1/2 teaspoon salt\n2 onions, finely chopped\n2 tablespoons tomato paste\n1/2 cup reduced-sodium beef broth\n1/2 cup water (plus an additional 1/2 cup water in slurry step)\n1 tablespoon cornstarch\n1 cup reduced-fat sour cream\n1 pound medium egg noodles\n1/2 cup chopped parsley (optional)",
                    Instructions = "1. Combine beef, mushrooms, onions, tomato paste, broth, and water in slow cooker. Cook on HIGH about 4 1/2 hours or LOW about 6 hours, until beef is tender.\n2. Stir together sour cream, remaining water, and cornstarch; add and cook on HIGH about 30 minutes, until thickened.\n3. Serve over egg noodles; sprinkle with parsley if desired.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 300,
                    Servings = 6,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Indian-Spiced Chicken Thighs (Slow Cooker)",
                    Description = "Tender slow cooker chicken thighs with garam masala, golden raisins, and a creamy yogurt sauce. Serve over basmati rice.",
                    Ingredients = "3 pounds boneless, skinless chicken thighs\n2 onions, thinly sliced\n3 garlic cloves, minced\n1/4 cup golden raisins\n1 3/4 teaspoons garam masala, divided\n1/2 teaspoon salt, divided\n1/4 teaspoon black pepper\n1 cup low-sodium chicken broth\n1/2 cup plain yogurt\n2 tablespoons cornstarch\n1/4 cup toasted slivered almonds\n2 cups cooked basmati rice (optional)",
                    Instructions = "1. Combine chicken, onions, garlic, raisins, 1 teaspoon garam masala, 1/4 teaspoon salt, pepper, and broth in slow cooker. Cover and cook for 3 hours on HIGH or 5 hours on LOW.\n2. In a small bowl, stir together remaining 3/4 teaspoon garam masala, remaining 1/4 teaspoon salt, yogurt, and cornstarch. Remove chicken to a platter; keep warm.\n3. Whisk yogurt mixture into slow cooker bowl and cover; cook an additional 15 minutes or until sauce has thickened. Stir in almonds and serve sauce with chicken over rice, if desired.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 195,
                    Servings = 4,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Black Beans and Pork (Slow Cooker)",
                    Description = "A hearty slow cooker dish with tender pork, black beans, and tomatoes with green chilies. Serve over rice.",
                    Ingredients = "1 1/2 pounds boneless pork loin, trimmed and cut into 1/2-inch cubes\n1 teaspoon chili powder\n1 teaspoon ground coriander\n1/4 teaspoon salt\n1/4 teaspoon black pepper\n1 onion, chopped\n2 cans (15 ounces each) black beans, drained and rinsed\n1 can (14.5 ounces) diced tomatoes with green chilies\n1 1/4 cups water\n1/4 cup chopped cilantro\n3 cups cooked white rice (optional)",
                    Instructions = "1. In a slow cooker bowl, toss together pork, chili powder, coriander, salt, and pepper. Stir in onion, beans, tomatoes, and 1 1/4 cups water. Cover and cook on HIGH for 4 hours or on LOW for 6 hours.\n2. Stir in cilantro. Using a potato masher, mash beans slightly until mixture is thickened. Serve bean mixture over rice, if desired.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 240,
                    Servings = 6,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                // Recipes from digitized_recipes_updated.pdf
                new Recipe
                {
                    Title = "Butterscotch Refrigerator Cookies",
                    Description = "Classic slice-and-bake refrigerator cookies with rich butterscotch flavor. Family recipe from Mom.",
                    Ingredients = "1 cup shortening\n1 1/4 cups brown sugar\n1 teaspoon vanilla\n2 eggs\n3 cups flour\n3 teaspoons baking powder\n1/2 teaspoon salt",
                    Instructions = "1. Cream shortening, brown sugar, and vanilla.\n2. Add eggs, one at a time.\n3. Sift together flour, baking powder, and salt; add to creamed mixture.\n4. Shape dough into rolls about 2 inches in diameter. Chill thoroughly.\n5. Slice into 1/8-inch slices and bake on an ungreased cookie sheet at 400°F for about 8 minutes.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 8,
                    Servings = 48,
                    Category = "Cookies",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Molasses Sugar Cookies",
                    Description = "Soft and chewy molasses cookies with warm spices. Family recipe from Mom.",
                    Ingredients = "3/4 cup shortening\n1 cup sugar\n1/4 cup molasses\n1 egg\n2 teaspoons baking soda\n2 cups flour\n1/2 teaspoon ginger\n1/2 teaspoon cloves\n1 teaspoon cinnamon\n1/2 teaspoon salt\nExtra sugar for rolling",
                    Instructions = "1. Melt the shortening; cool.\n2. Add sugar, molasses, and egg; mix well.\n3. Sift together baking soda, flour, ginger, cloves, cinnamon, and salt; add to wet mixture.\n4. Chill dough.\n5. Form into 1-inch balls; roll in sugar.\n6. Bake at 375°F for 8–10 minutes or until set.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 10,
                    Servings = 36,
                    Category = "Cookies",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Peanut Butter Cookies (Betty Crocker)",
                    Description = "Classic peanut butter cookies with the traditional crisscross fork pattern. About 3 dozen cookies.",
                    Ingredients = "1/2 cup shortening (or half butter)\n1/2 cup peanut butter\n1/2 cup sugar\n1/2 cup brown sugar\n1 egg\n1 1/4 cups sifted flour\n1/2 teaspoon baking powder\n3/4 teaspoon baking soda\n1/4 teaspoon salt",
                    Instructions = "1. Cream shortening, peanut butter, and sugars.\n2. Add egg and mix well.\n3. Sift together flour, baking powder, baking soda, and salt; add to creamed mixture.\n4. Chill dough.\n5. Roll into balls.\n6. Place about 3 inches apart on a greased baking sheet.\n7. Flatten with a fork in a crisscross pattern.\n8. Bake at 375°F for 10–12 minutes.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 12,
                    Servings = 36,
                    Category = "Cookies",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Peanut Butter Blossoms",
                    Description = "Classic peanut butter cookies topped with Hershey's Kisses. A holiday favorite!",
                    Ingredients = "HERSHEY'S KISSES Milk Chocolates (unwrapped)\n1/2 cup shortening\n1/2 cup creamy peanut butter\n1/2 cup granulated sugar\n1/2 cup packed light brown sugar\n1 egg\n2 tablespoons milk\n1 teaspoon vanilla extract\n1 1/2 cups all-purpose flour\n1 teaspoon baking soda\n1/2 teaspoon salt\n1/3 cup granulated sugar (for rolling)",
                    Instructions = "1. Heat oven to 375°F. Remove wrappers from chocolates.\n2. Beat shortening and peanut butter in a large bowl until well blended.\n3. Add 1/2 cup granulated sugar and brown sugar; beat until fluffy.\n4. Add egg, milk, and vanilla; beat well.\n5. Stir together flour, baking soda, and salt; gradually beat into peanut butter mixture.\n6. Shape dough into 1-inch balls. Roll in granulated sugar; place on ungreased cookie sheet.\n7. Bake 8–10 minutes or until lightly browned.\n8. Immediately press a chocolate into the center of each cookie (cookie will crack around edges).\n9. Remove to a wire rack; cool completely.",
                    PrepTimeMinutes = 25,
                    CookTimeMinutes = 10,
                    Servings = 48,
                    Category = "Cookies",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Thumbprint Cookies",
                    Description = "Buttery cookies with a jam-filled center. Recipe from SugarSpunRun.com. Makes 24 cookies.",
                    Ingredients = "1 cup unsalted butter, softened (226g)\n1/2 cup sugar (70g)\n1/2 cup light brown sugar, lightly packed (70g)\n3/4 teaspoon vanilla extract\n1 large egg yolk\n2 1/2 cups all-purpose flour (280g)\n2 teaspoons cornstarch\n1/2 teaspoon salt\n1/2 cup sugar for rolling (optional)\n1/2 cup jam or preserves (raspberry recommended)",
                    Instructions = "1. Beat butter until creamy.\n2. Add sugars; beat until combined (about 30–60 seconds).\n3. Add egg yolk and vanilla; beat well.\n4. Whisk together flour, cornstarch, and salt.\n5. Gradually add flour mixture to wet ingredients until combined. Use your hands to finish working the dough together.\n6. Scoop cookie dough into 1 tablespoon-sized balls and roll well (so the dough is round with no cracks).\n7. Roll in granulated sugar (if using) and place on a wax-paper covered plate. Use your thumb or the rounded back of a teaspoon to gently press an indent in the center of each ball.\n8. Transfer cookie dough to freezer and chill for 30 minutes.\n9. Preheat oven to 375°F. Heat jam briefly in microwave until easy to stir.\n10. Spoon jam into each thumbprint, filling each indent to the brim.\n11. Place cookies at least 2 inches apart on a parchment paper-lined cookie sheet and bake at 375°F for 11 minutes or until edges are just beginning to turn golden brown.\n12. Allow cookies to cool completely on baking sheet before enjoying.",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 11,
                    Servings = 24,
                    Category = "Cookies",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "My Favorite Butter Cookies",
                    Description = "Rich almond-flavored butter cookies perfect for cookie press or cut-outs. Makes about 6 dozen. Don't use instant flour!",
                    Ingredients = "1 1/2 cups butter\n1 cup sugar\n1 egg\n1 teaspoon almond extract\n4 cups flour (don't use instant flour)\n1 teaspoon baking powder\nFood coloring (optional)",
                    Instructions = "1. Heat oven to 375°F.\n2. Cream butter and sugar.\n3. Add egg and almond extract (and food coloring, if using).\n4. Add flour and baking powder; mix to form dough.\n5. Shape cookies using cookie press or cut-outs.\n6. Bake on an ungreased cookie sheet for 6–8 minutes or until set.",
                    PrepTimeMinutes = 25,
                    CookTimeMinutes = 8,
                    Servings = 72,
                    Category = "Cookies",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                // Recipes from digitized_recipes (1).pdf
                new Recipe
                {
                    Title = "Ranger Cookies",
                    Description = "Crispy cookies loaded with oats, Rice Krispies, and coconut. Family recipe from Mom. Makes 3 dozen.",
                    Ingredients = "1 cup butter\n1 cup white sugar\n1 cup brown sugar\n2 eggs\n1 teaspoon vanilla\n2 cups flour\n1 teaspoon baking soda\n1/2 teaspoon baking powder\n1/2 teaspoon salt\n2 cups quick oats\n2 cups Rice Krispies\n1 cup shredded coconut",
                    Instructions = "1. Cream butter, white sugar, and brown sugar well. Add eggs and vanilla; mix until smooth.\n2. Sift together flour, baking soda, baking powder, and salt. Add to creamed mixture and mix.\n3. Stir in oats, Rice Krispies, and shredded coconut.\n4. Dough is crumbly. Mold into balls the size of a walnut and place on a greased cookie sheet.\n5. Press each ball with a fork. Bake at 350°F until done.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 12,
                    Servings = 36,
                    Category = "Cookies",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Peanut Butter Chip Chocolate Cookies",
                    Description = "Rich chocolate cookies studded with peanut butter chips. Recipe from Betty. Makes about 3 dozen.",
                    Ingredients = "1 cup margarine\n1 1/2 cups sugar\n2 eggs\n2 teaspoons vanilla\n2 cups flour\n2/3 cup cocoa\n3/4 teaspoon baking soda\n1/2 teaspoon salt\n12 ounces peanut butter chips",
                    Instructions = "1. Cream margarine and sugar. Add eggs and vanilla; mix well.\n2. Combine flour, cocoa, baking soda, and salt; add to creamed mixture and mix.\n3. Stir in peanut butter chips.\n4. Drop by teaspoonful onto an ungreased cookie sheet.\n5. Bake at 350°F for 8–10 minutes.\n6. Cool 1 minute before removing from cookie sheet.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 10,
                    Servings = 36,
                    Category = "Cookies",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Hamburger-Bean Casserole",
                    Description = "A quick and easy ground beef and bean casserole with tangy sauce. Family recipe from Mom.",
                    Ingredients = "1 tablespoon butter\n1 pound ground beef\n1 package onion soup mix\n1/2 cup water\n1 cup ketchup\n2 tablespoons prepared mustard\n2 tablespoons vinegar\n1 (14-ounce) can pork and beans",
                    Instructions = "1. Brown ground beef in butter.\n2. Add remaining ingredients and combine.\n3. Bake at 400°F for 20–30 minutes.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 30,
                    Servings = 6,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Tuna-Kraft Dinner Casserole",
                    Description = "A comforting casserole using macaroni and cheese dinner, tuna, and tomatoes. Family recipe from Mom. Serves 5.",
                    Ingredients = "3 (7 1/2-ounce) packages macaroni & cheese dinner\n1 tablespoon soft margarine\n1 (7 1/2-ounce) can tomatoes (about 1 cup)\n1/2 cup milk\n2 tablespoons instant minced onion\n1 egg, slightly beaten\n1 can tuna (6 1/2 to 9 1/4 ounces)\n2 tablespoons snipped parsley\n1/4 teaspoon salt\nDash of pepper\n2 tablespoons cornflake crumbs",
                    Instructions = "1. Cook macaroni according to package directions; drain.\n2. Add cheese (from the dinner packages) and margarine; toss to mix.\n3. Drain tomatoes, reserving liquid. Dice tomatoes.\n4. Add diced tomatoes, reserved liquid, and remaining ingredients except crumbs; mix.\n5. Pour into a greased 1-quart casserole.\n6. Sprinkle with cornflake crumbs.\n7. Bake uncovered at 350°F for 35 minutes.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 35,
                    Servings = 5,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Turkey Sausage and Spicy Tomato Sauce (Slow Cooker)",
                    Description = "A flavorful slow cooker pasta sauce with smoked turkey sausage and aromatic spices. From Cooking Light (Oct 2014). Serves 12.",
                    Ingredients = "1 tablespoon olive oil\n1 1/4 pounds smoked turkey sausage, cut into 1-inch pieces\n2 cups chopped onion\n15 garlic cloves, chopped (about 1 head)\n2 carrots, very thinly sliced\n2 Fresno chiles, thinly sliced\n3 tablespoons chopped fresh thyme\n2 teaspoons ground coriander\n2 teaspoons ground cumin\n1 teaspoon ground cinnamon\n1 teaspoon ground turmeric\n1/2 teaspoon crushed red pepper\n1 cup dry white wine\n2 (28.5-ounce) cans unsalted whole tomatoes\n1 tablespoon mustard seeds\n1 tablespoon lower-sodium soy sauce\n6 (2-inch) strips lemon rind\n3 bay leaves\n8 cups cooked penne pasta\n1/4 cup chopped fresh cilantro\n2 ounces Cotija cheese, crumbled",
                    Instructions = "1. Heat a large Dutch oven over medium-high heat. Add oil; swirl to coat. Add sausage; cook 8 minutes or until browned, stirring occasionally. Transfer sausage to a 6-quart slow cooker.\n2. Add onion, garlic, carrots, and chiles to pan; sauté 6 minutes or until tender. Add thyme, coriander, cumin, cinnamon, turmeric, and crushed red pepper; cook 1 minute. Add wine; bring to a boil. Cook 2 minutes or until wine is reduced by half. Add onion mixture to slow cooker.\n3. Drain 1 can of tomatoes. Add drained tomatoes, remaining 1 can tomatoes with liquid, mustard seeds, soy sauce, lemon rind, and bay leaves to slow cooker. Stir to combine and break up tomatoes. Cook on LOW for 8 hours. Remove and discard rind and bay leaves.\n4. Serve sauce over pasta; sprinkle with cilantro and Cotija cheese.",
                    PrepTimeMinutes = 25,
                    CookTimeMinutes = 480,
                    Servings = 12,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Stay-Abed Stew",
                    Description = "An easy slow-baked beef stew that cooks while you relax. From Family Circle (9/1/82). Serves 5-6.",
                    Ingredients = "2 pounds beef stew meat, cubed\n1 can tiny peas (optional)\n1 cup sliced carrots\n2 onions, chopped\n1 teaspoon salt\nDash of pepper\n1 can cream of celery soup (or mushroom or tomato), thinned with 1/2 can water\n1 big raw potato, sliced\nPiece of bay leaf",
                    Instructions = "1. Mix everything together in a tight-lidded casserole dish.\n2. Cover and bake at 275°F for 5 hours.",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 300,
                    Servings = 6,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Stew (Tapioca-Thickened)",
                    Description = "A hearty beef stew thickened with tapioca. Can be made in oven or crockpot.",
                    Ingredients = "8 or more potatoes, diced and peeled\n1 pound baby carrots (or more)\n4 or more celery stalks, cut up\n3/4 teaspoon onion powder\n3 tablespoons Minute tapioca\n1 tablespoon sugar\n1 tablespoon salt\n2 pounds stewing beef, cubed\n1 can cut-up tomatoes\n1/2 cup water",
                    Instructions = "1. Layer vegetables in the bottom of a crockpot or Dutch oven.\n2. Mix onion powder, Minute tapioca, sugar, and salt; sprinkle over layers of vegetables, then over beef.\n3. Put beef on top of vegetables.\n4. Pour tomatoes and water over top of all.\n5. Cover and cook: 5 hours at 250°F, or 2 1/2 hours at 300°F, or all day in a crockpot.\n6. Stir when nearly done.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 300,
                    Servings = 8,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "5-Hour Stew",
                    Description = "A simple slow-cooked beef stew thickened with tapioca. Family recipe from Mom.",
                    Ingredients = "2 pounds cubed stewing meat\n3 potatoes, diced\n3 carrots, diced\n1 large onion, diced\n1 can tomatoes (about 2 cups)\n1/2 cup water\n3 tablespoons Minute tapioca\n1 tablespoon sugar\nSalt and pepper to taste\nSmall amount of shortening (for browning)",
                    Instructions = "1. Brown meat in a small amount of shortening.\n2. Combine with remaining ingredients and turn into a covered casserole.\n3. Bake at 250°F for 5 hours.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 300,
                    Servings = 6,
                    Category = "Main Dishes",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Recipe
                {
                    Title = "Zucchini Bread",
                    Description = "Moist and delicious zucchini bread with cinnamon and nuts. Recipe from Sharon Fuller. Makes 2 loaves.",
                    Ingredients = "3 eggs, beaten\n1/2 cup cooking oil\n2 cups sugar\n2 cups peeled and grated zucchini\n2 teaspoons vanilla\n3 cups flour\n1 teaspoon baking soda\n1 teaspoon baking powder\n1 teaspoon salt\n1 teaspoon cinnamon\n1/2 cup chopped nuts",
                    Instructions = "1. Grease and flour 2 loaf pans.\n2. Cream together oil, eggs, sugar, zucchini, and vanilla.\n3. In a separate bowl, mix flour, baking soda, baking powder, salt, and cinnamon.\n4. Add dry ingredients to creamed mixture; mix thoroughly until blended.\n5. Add nuts.\n6. Bake at 325°F for 45 minutes.",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 45,
                    Servings = 24,
                    Category = "Breads",
                    ImageUrl = null,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
