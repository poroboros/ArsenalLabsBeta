using System.Collections.Generic;
using System.IO;
using ArsenalLabsBeta.Items.Ammo.Lens;
using ArsenalLabsBeta.Items.Mounts;
using ArsenalLabsBeta.Items.Weapons.ArsenalWhip;
using ArsenalLabsBeta.Items.Weapons.Bloomhammer;
using ArsenalLabsBeta.Items.Weapons.Florapedia;
using ArsenalLabsBeta.Items.Weapons.Melee;
using ArsenalLabsBeta.Items.Weapons.Ranged.Repeaters;
using CalamityMod;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.LabFinders;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
//alias for ease of use 
using CSchem = ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalLabsSchematicLoaderSystem;

namespace ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalFactory;

public class SurfaceLab : ModSystem
{
    public const int FactoryWidth = 118;
    public const int FactoryHeight = 263;

    public const int Arena1Depth = 150;
    public const int Arena2Depth = 245; //-5

    public const int ArenaHeight = 14;

    public static int FactoryX;
    public static int FactoryY;
    public static bool SuppressionActive = true;
    public static bool HasFactory = true;

    public static Rectangle MainFactoryBounds;
    public static Rectangle Arena1Bounds;
    public static Rectangle Arena2Bounds;
    public static Rectangle LeftTreasureCaveBounds;
    public static Rectangle RightTreasureCaveBounds;
    public static Rectangle CoreCaveBounds;

    /// <summary>
    /// The ID for the important item to place in this chamber
    /// </summary>
    public static int ImportantItemID = 0;

    public static List<TestChamberSpawnData> TestChambersLeft = new()
    {
        new TestChamberSpawnData("LavaFall_left"),
        new TestChamberSpawnData("LavaPuddle_left", new Point(0, 1)),
        new TestChamberSpawnData("LavaTestChamber_left"),
        new TestChamberSpawnData("SlipAndSlide_left"),
        new TestChamberSpawnData("InvisibleMaze_left", new Point(0, 0)),

        new TestChamberSpawnData("BathAndPortals_left"),
        new TestChamberSpawnData("Beds_left", new Point(0,1)),
        new TestChamberSpawnData("TurretAndGrid_left", new Point(0,1)),
    };

    public static List<TestChamberSpawnData> TestChambersRight = new()
    {
        new TestChamberSpawnData("PlatformChallenge_right", new Point(0, -1)),
        new TestChamberSpawnData("ElectricalPanels_right", new Point(0, 0)),
        new TestChamberSpawnData("LogicalRoom_right", new Point(0, 0)),
        new TestChamberSpawnData("BunnyRoom_right"),

        new TestChamberSpawnData("LavaSwitches_right"),
        new TestChamberSpawnData("SwitchesAndTurrets_right"),
        new TestChamberSpawnData("SuppressionGridCapsule_right", new Point(0,-1)),
    };

    public static List<TestChamberSpawnData> TestChambersLeftTall = new()
    {
        new TestChamberSpawnData("LavaTank_lefttall"),
        new TestChamberSpawnData("CoreParkour_lefttall"),
        new TestChamberSpawnData("RustCreeping_lefttall"),
        new TestChamberSpawnData("TurretParkour_lefttall"),

    };

    //Positions where random test chambers should go
    private const int RoomxLeftOffset = -51;
    private const int RoomxRightOffset = 3;

    private static readonly List<Point> LeftChamberPoints = new()
    {
        new Point(RoomxLeftOffset, 43),
        new Point(RoomxLeftOffset, 81),
        //new Point(roomxLeftOffset,119),
        new Point(RoomxLeftOffset, 195),
        new Point(RoomxLeftOffset, 233)
    };

    private static readonly List<Point> RightChamberPoints = new()
    {
        new Point(RoomxRightOffset, 25),
        new Point(RoomxRightOffset, 82),
        new Point(RoomxRightOffset, 119 + 90 - 13),
        new Point(RoomxRightOffset, 139 + 90 - 14)
    };


    public void ResetFlags()
    {
        FactoryX = -1;
        FactoryY = -1;
        SuppressionActive = true;
        HasFactory = false;
    }

    public override void OnWorldLoad()
    {
        ResetFlags();
    }

    public override void OnWorldUnload()
    {
        ResetFlags();
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["HasFactory"] = HasFactory;
        tag["FactoryX"] = FactoryX;
        tag["FactoryY"] = FactoryY;
        tag["SuppressionActive"] = SuppressionActive;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        FactoryX = tag.Get<int>("FactoryX");
        FactoryY = tag.Get<int>("FactoryY");
        HasFactory = tag.Get<bool>("HasFactory");
        SuppressionActive = tag.Get<bool>("SuppressionActive");

        MainFactoryBounds = new Rectangle(FactoryX - FactoryWidth / 2, FactoryY,
            FactoryWidth, FactoryHeight);

        Arena1Bounds = new Rectangle(FactoryX - FactoryWidth / 2, FactoryY + Arena1Depth,
            FactoryWidth, ArenaHeight);

        LeftTreasureCaveBounds = new Rectangle(FactoryX - FactoryWidth / 2 - 100, FactoryY + Arena1Depth - 10,
            100, 45);

        Arena2Bounds = new Rectangle(FactoryX - FactoryWidth / 2, FactoryY + Arena2Depth,
            FactoryWidth, ArenaHeight);

        RightTreasureCaveBounds = new Rectangle(FactoryX + FactoryWidth / 2, FactoryY + Arena1Depth - 30,
            150, 100);

        CoreCaveBounds = new Rectangle(FactoryX - FactoryWidth / 2 - 30, FactoryY + FactoryHeight,
            170, 185);
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(FactoryX);
        writer.Write(FactoryY);
        writer.Write(SuppressionActive);
    }

    public override void NetReceive(BinaryReader reader)
    {
        FactoryX = reader.ReadInt32();
        FactoryY = reader.ReadInt32();
        SuppressionActive = reader.ReadBoolean();
    }

    /// <summary>
    ///     Organizes all the surface lab rooms into their proper lists, as well as loading them into calamity
    ///     This relies heavily on directory placement for automatically loading these schematics, so be careful moving stuff
    /// </summary>
    /// <param name="startDir">The directory containing all the subdirs</param>
    public static void LoadRoomSchematics(string startDir)
    {
        //CSchem.TileMaps["LavaTank_lefttall"] = CSchem.LoadExternalSchematic(startDir + "LavaTank_lefttall" + ".csch");

        foreach (var room in TestChambersLeft)
            CSchem.TileMaps[room.name] = CSchem.LoadExternalSchematic(startDir + room.name + ".csch");

        foreach (var room in TestChambersLeftTall)
            CSchem.TileMaps[room.name] = CSchem.LoadExternalSchematic(startDir + room.name + ".csch");

        foreach (var room in TestChambersRight)
            CSchem.TileMaps[room.name] = CSchem.LoadExternalSchematic(startDir + room.name + ".csch");
    }

    public static void PlaceSurfaceLab(GenerationProgress progress, GameConfiguration _2)
    {
        #region Determine Position
        //Place the main part of the lab
        var mainlabName = "ArsenalLabMain";


        //copy room name lists to be safe!
        var lRoomCopy = new List<TestChamberSpawnData>(TestChambersLeft.Count);

        TestChambersLeft.ForEach(item => { lRoomCopy.Add(item); });

        var rRoomCopy = new List<TestChamberSpawnData>(TestChambersRight.Count);

        TestChambersRight.ForEach(item => { rRoomCopy.Add(item); });

        var lTRoomCopy = new List<TestChamberSpawnData>(TestChambersRight.Count);

        TestChambersLeftTall.ForEach(item => { lTRoomCopy.Add(item); });


        var isLarge = WorldGen.GetWorldSize() == WorldGen.WorldSize.Large;

        //medium vs large, adjust if it keeps clipping into the ground weirdly
        var yOffset = isLarge ? 100 : 75;

        //reduce offset so it doesnt generate inside hell in gfb, very important since if it goes too low on medium worlds the game could crash
        if (Main.remixWorld)
            yOffset = isLarge ? 75 : 55;
        var xOffset = 0;

        float worldSpawnCoord = Main.spawnTileX;
        var xOrigin = (int)worldSpawnCoord - 400;


        //scan for glowing mushroom biomes to place above
        //make sure it's not in the caverns 
        //Also avoid these biomes: ice, jungle, spawn
        foreach (var candidatePoint in GenVars.mushroomBiomesPosition)
        {
            if (candidatePoint.X == 0)
                break;
            var inCavern =  candidatePoint.Y > GenVars.rockLayerHigh + 400;
            var inIce = candidatePoint.X < GenVars.snowMaxX[0] + 250 && candidatePoint.X > GenVars.snowMinX[0] - 250;


            //bool inJungle = candidatePoint.X < GenVars.jungleMaxX + 300 && candidatePoint.X > GenVars.jungleMinX - 300;


            var inSpawn = candidatePoint.X < worldSpawnCoord + (isLarge? 750 : 500) && candidatePoint.X > worldSpawnCoord - (isLarge ? 750 : 500);

            var inDungeon = candidatePoint.X < Main.dungeonX + 500 && candidatePoint.X > Main.dungeonX - 500;

            var leftBorder = Main.leftWorld / 16;
            var rightBorder = Main.rightWorld / 16;


            float rightWorldDistance = rightBorder - candidatePoint.X;
            float leftWorldDistance = candidatePoint.X - leftBorder;

            //ArsenalLabsBeta.Instance.Logger.Info("rW " + rightWorldDistance + " - lW " + leftWorldDistance);

            var closeToBorders = leftWorldDistance < 400 || rightWorldDistance < 400;


            if ((inCavern && !Main.remixWorld) || (!inCavern && Main.remixWorld) || closeToBorders || inSpawn || inDungeon) continue;

            var tempOffset = 0;


            //dodges jungle snow and desert if possible
            float jungleLeftDistance = GenVars.jungleMinX - candidatePoint.X;
            float jungleRightDistance = candidatePoint.X - GenVars.jungleMaxX;

            if (jungleLeftDistance < 200) tempOffset = -200;

            if (jungleRightDistance < 200) tempOffset = 200;

            float snowLeftDistance = GenVars.snowMinX[0] - candidatePoint.X;
            float snowRightDistance = candidatePoint.X - GenVars.snowMaxX[0];

            if (snowLeftDistance < 200 && snowRightDistance < -200) tempOffset = -200;

            if (snowRightDistance > 200 && snowLeftDistance < -200) tempOffset = 200;

            float desLeftDistance = GenVars.desertHiveLeft - candidatePoint.X;
            float desRightDistance = candidatePoint.X - GenVars.desertHiveRight;


            if (desLeftDistance < 300 && desRightDistance < -300) tempOffset = 0;

            if (desRightDistance > 300 && desLeftDistance < -300) tempOffset = 0;



            if (rightWorldDistance < 800) tempOffset = -350;


            if (leftWorldDistance < 800) tempOffset = 350;


            if (jungleRightDistance < 0 && jungleLeftDistance < 0)
                continue;
            if (snowLeftDistance < 0 && snowRightDistance < 0)
                continue;
            if (desLeftDistance < 0 && desRightDistance < 0)
                continue;

            xOffset = tempOffset;
            xOrigin = candidatePoint.X;
            break;
        }


        //Determine placement position
        var placementPositionX = xOrigin + xOffset;
        var placementPositionY = (int)Main.worldSurface - yOffset;

        // Generates in the "surface"
        if (Main.remixWorld)
            placementPositionY = (int)Main.rockLayer + yOffset;

        Point mainPlacementPoint = new(placementPositionX, placementPositionY);


        //these are used to not put underground
        var doubleCheck = new Point(mainPlacementPoint.X + 50, mainPlacementPoint.Y);
        var tripleCheck = new Point(mainPlacementPoint.X - 50, mainPlacementPoint.Y);

        //Place on the ground

        var maxExtension = isLarge ? 75 : 50;

        // Bypass this in remix and gfb
        while (Main.tile[mainPlacementPoint].IsTileSolidGround() || Main.tile[doubleCheck].IsTileSolidGround() ||
               Main.tile[tripleCheck].IsTileSolidGround() && !Main.remixWorld)
        {
            mainPlacementPoint.Y -= 1;
            doubleCheck.Y -= 1;
            tripleCheck.Y -= 1;


            //Make sure it's not beyond a certain height
            maxExtension -= 1;

            if (maxExtension < 0)
                break;
        }
        //GenVars.snow

        #endregion

        #region Place rooms

        //place the items in main
        var tileMap = CSchem.TileMaps[mainlabName];
        var anchorType = SchematicAnchor.TopCenter;
        // If it has given a weapon or not, used on later ones but it has to be here or the Schematic Manager gets angry
        // The first chest will not have a weapon, while the rest will
        bool firstItem = false;
        ImportantItemID = 0; // Important to reset, especially when making many worlds
        ArsenalLabsBeta.Instance.Logger.Info("Placing main");
        SchematicManager.PlaceSchematic(mainlabName, mainPlacementPoint, anchorType, ref firstItem,
            FillMainChests);


        //place bottom of the lab
        var bottomLabName = "ArsenalLabBottom";
        Point bottomPlacementPoint = new(mainPlacementPoint.X - 58 + 27, mainPlacementPoint.Y + 328 - 67);
        var tileMap3 = CSchem.TileMaps[bottomLabName];
        ArsenalLabsBeta.Instance.Logger.Info("Placing bottom");
        firstItem = true; //Generates at the bottom so can have weapons
        SchematicManager.PlaceSchematic(bottomLabName, bottomPlacementPoint, anchorType, ref firstItem,
            FillLabChests);


        //place BarrierGenerator
        var barrierGenerator = "BarrierGenerator";
        Point barrierPlacementPoint = new(bottomPlacementPoint.X + 7, bottomPlacementPoint.Y + 150 - 18);
        //var tileMap4 = CSchem.TileMaps[barrierGenerator];
        ArsenalLabsBeta.Instance.Logger.Info("Placing barrier gen");
        firstItem = true; //Generates at the bottom so can have weapons
        // Maybe this part could have better loot?
        SchematicManager.PlaceSchematic(barrierGenerator, barrierPlacementPoint, anchorType, ref firstItem,
            FillLabChests);


        //add the tall room
        var randIdx1 = WorldGen._genRand.Next(0, lTRoomCopy.Count);
        var ltRoom = lTRoomCopy[randIdx1];
        firstItem = false; // Too high up for a weapon
        PlaceRoom(progress, mainPlacementPoint + new Point(RoomxLeftOffset, 110) + ltRoom.offset, ltRoom.name,
            SchematicAnchor.Center, firstItem);

        
        // Place left side chambers
        firstItem = false; // Reset so no free weapon in the literal first room
        foreach (var rPoint in LeftChamberPoints)
        {
            //randomly selected
            var randIdx = WorldGen._genRand.Next(0, lRoomCopy.Count);

            var rRoom = lRoomCopy[randIdx];
            lRoomCopy.RemoveAt(randIdx);

            PlaceRoom(progress, rPoint + mainPlacementPoint + rRoom.offset, rRoom.name, SchematicAnchor.Center, firstItem);
        }

        // Place right side chambers
        firstItem = false; // Reset so no free weapon in the literal first room
        foreach (var rPoint in RightChamberPoints)
        {
            var randIdx = WorldGen._genRand.Next(0, rRoomCopy.Count);

            var rRoom = rRoomCopy[randIdx];
            rRoomCopy.RemoveAt(randIdx);

            PlaceRoom(progress, rPoint + mainPlacementPoint + rRoom.offset, rRoom.name, SchematicAnchor.Center, firstItem);
        }
        

        var clearanceName = "LabAboveClearance";
        Point clearancePoint = new(mainPlacementPoint.X - 51 + 27, mainPlacementPoint.Y);
        var tileMapC = CSchem.TileMaps[clearanceName];
        SchematicManager.PlaceSchematic(clearanceName, clearancePoint, SchematicAnchor.BottomCenter,
            ref firstItem, FillLabChests);


        //place the top (after chambers to prevent clipping)
        var topLabName = "ArsenalLabTop";
        Point point2 = new(mainPlacementPoint.X - 51 + 27, mainPlacementPoint.Y - 37);
        var tileMap2 = CSchem.TileMaps[topLabName];
        bool seekingMechanism = false;
        SchematicManager.PlaceSchematic(topLabName, point2, anchorType, ref seekingMechanism, FillTopChests);

        #endregion

        //set up schematic protection
        var schematicProtectionArea = CalamityUtils.GetSchematicProtectionArea(tileMap, mainPlacementPoint, anchorType);
        var schematicProtectionArea2 = CalamityUtils.GetSchematicProtectionArea(tileMap2, point2, anchorType);
        var schematicProtectionArea3 =
            CalamityUtils.GetSchematicProtectionArea(tileMap3, bottomPlacementPoint, anchorType);

        var rect2 = Rectangle.Union(schematicProtectionArea, schematicProtectionArea2);
        rect2 = Rectangle.Union(rect2, schematicProtectionArea3);
        GenVars.structures.AddProtectedStructure(rect2, 30);


        //save biome location to the world
        FactoryX = mainPlacementPoint.X - 24;
        FactoryY = mainPlacementPoint.Y;
        HasFactory = true;
    }

    public static void PlaceRoom(GenerationProgress progress, Point point, string roomName, SchematicAnchor anchorType, bool firstItem)
    {
        var tileMap = CSchem.TileMaps[roomName];

        if (roomName == "InvisibleMaze_left")
            point.Y += 1;
        var vector2 = new Vector2(tileMap.GetLength(0), tileMap.GetLength(1));
        SchematicManager.PlaceSchematic(roomName, point, anchorType, ref firstItem, FillLabChests);
    }

    public static void DebugDrawBounds()
    {
        Dust.NewDust(new Vector2(FactoryX, FactoryY) * 16, 5, 5, DustID.RedTorch);


        var worldRec = new Rectangle(MainFactoryBounds.X * 16, MainFactoryBounds.Y * 16, MainFactoryBounds.Width * 16,
            MainFactoryBounds.Height * 16);

        var ar1wRec = new Rectangle(Arena1Bounds.X * 16, Arena1Bounds.Y * 16, Arena1Bounds.Width * 16,
            Arena1Bounds.Height * 16);

        var tr1wRec = new Rectangle(RightTreasureCaveBounds.X * 16, RightTreasureCaveBounds.Y * 16,
            RightTreasureCaveBounds.Width * 16, RightTreasureCaveBounds.Height * 16);

        var tr2wRec = new Rectangle(LeftTreasureCaveBounds.X * 16, LeftTreasureCaveBounds.Y * 16,
            LeftTreasureCaveBounds.Width * 16, LeftTreasureCaveBounds.Height * 16);

        var ar2wRec = new Rectangle(Arena2Bounds.X * 16, Arena2Bounds.Y * 16, Arena2Bounds.Width * 16,
            Arena2Bounds.Height * 16);

        var cwRec = new Rectangle(CoreCaveBounds.X * 16, CoreCaveBounds.Y * 16, CoreCaveBounds.Width * 16,
            CoreCaveBounds.Height * 16);


        Utils.DrawRect(Main.spriteBatch, worldRec, Color.SkyBlue);
        Utils.DrawRect(Main.spriteBatch, ar1wRec, Color.Red);
        Utils.DrawRect(Main.spriteBatch, ar2wRec, Color.Gold);
        Utils.DrawRect(Main.spriteBatch, tr1wRec, Color.Pink);
        Utils.DrawRect(Main.spriteBatch, tr2wRec, Color.Purple);
        Utils.DrawRect(Main.spriteBatch, cwRec, Color.Green);
    }

    #region Chests
    public static void FillLabChests(Chest chest, int Type, bool firstItem)
    {
        // This is the main item, I need to see if its ok with the others
        int type1 = Utils.SelectRandom(WorldGen.genRand,
            ModContent.ItemType<Florapedia>(), ModContent.ItemType<BlossomingGale>(),
            ModContent.ItemType<ArsenalRepeater>(), ModContent.ItemType<Bloomhammer>(), ModContent.ItemType<ArsenalWhip>());
        // Random Ingot Bar
        int type2 = Utils.SelectRandom(WorldGen.genRand,
            ItemID.GoldBar, ItemID.PlatinumBar,
            ItemID.IronBar, ItemID.LeadBar,
            ItemID.SilverBar, ItemID.TungstenBar,
            ItemID.CopperBar, ItemID.TinBar);
        //Random Potion
        int type3 = Utils.SelectRandom(WorldGen.genRand,
            ItemID.ThornsPotion, ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.HunterPotion, ItemID.SpelunkerPotion, ItemID.TrapsightPotion,
            ItemID.WaterWalkingPotion, ModContent.ItemType<CalciumPotion>());
        List<ChestItem> chestItemList = new List<ChestItem>() { 
            new ChestItem(type1, 1),
            new ChestItem(type2, WorldGen.genRand.Next(4, 7)),
            new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(2, 5)),
            new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(2, 5)),
            new ChestItem(ItemID.Torch, WorldGen.genRand.Next(2, 5)),
            new ChestItem(type3, WorldGen.genRand.Next(1, 4)),
            new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(1, 3))
          };
        if (!firstItem)
        {
            chestItemList.RemoveAt(0);
        }
        // Adds a 50% chance for a bonus item, and then rolls for both, checks if the chest already has one of em guaranteed
        // I need to check if adding these is ok
        float rng = WorldGen.genRand.NextFloat();
        if (rng >= 0.5f && rng < 0.75f && ImportantItemID != 3)
            chestItemList.Insert(1, new ChestItem(ModContent.ItemType<ApertureCore>(), 1));
        else if (rng >= 0.75f && ImportantItemID != 4)
            chestItemList.Insert(1, new ChestItem(ModContent.ItemType<SpelunkerChip>(), 1));
        // Add Juggernuts if on Gfb at a 33% chance
        if (Main.zenithWorld && rng < 0.33f)
            chestItemList.Insert(7, new ChestItem(ModContent.ItemType<Juggernut>(), WorldGen.genRand.Next(1, 3)));
        for (int index = 0; index < chestItemList.Count; ++index)
        {
            chest.item[index].SetDefaults(chestItemList[index].Type);
            chest.item[index].stack = chestItemList[index].Stack;
        }
    }

    public static void FillMainChests(Chest chest, int Type, bool firstItem)
    {
        // This is the main item, I need to see if its ok with the others
        int type1 = Utils.SelectRandom(WorldGen.genRand,
            ModContent.ItemType<Florapedia>(), ModContent.ItemType<BlossomingGale>(),
            ModContent.ItemType<ArsenalRepeater>(), ModContent.ItemType<Bloomhammer>(), ModContent.ItemType<ArsenalWhip>());
        // Random Ingot Bar, guaranteed higher tier bars
        int type2 = Utils.SelectRandom(WorldGen.genRand,
            ItemID.GoldBar, ItemID.PlatinumBar,
            ItemID.SilverBar, ItemID.TungstenBar);

        //Random Potion
        int type3 = Utils.SelectRandom(WorldGen.genRand,
            ItemID.ThornsPotion, ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.HunterPotion, ItemID.SpelunkerPotion, ItemID.TrapsightPotion,
            ItemID.WaterWalkingPotion, ModContent.ItemType<CalciumPotion>());

        List<ChestItem> chestItemList = new List<ChestItem>()
          {
            new ChestItem(type1, 1),
            new ChestItem(type2, WorldGen.genRand.Next(4, 7)),
            new ChestItem(ModContent.ItemType<DubiousPlating>(), WorldGen.genRand.Next(2, 5)),
            new ChestItem(ModContent.ItemType<MysteriousCircuitry>(), WorldGen.genRand.Next(2, 5)),
            new ChestItem(ItemID.Torch, WorldGen.genRand.Next(2, 5)),
            new ChestItem(type3, WorldGen.genRand.Next(1, 4)),
            new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(1, 3))
          };
        // Room specific Items
        // REMEMBER: Generation is from left to right
        switch (ImportantItemID)
        {
            // These 2 are left side after defeating trial 1, I assume an armory
            case 0:
            case 1:
                //Adds an extra weapon
                int type4 = Utils.SelectRandom(WorldGen.genRand,
                    ModContent.ItemType<Florapedia>(), ModContent.ItemType<BlossomingGale>(),
                    ModContent.ItemType<ArsenalRepeater>(), ModContent.ItemType<Bloomhammer>(), ModContent.ItemType<ArsenalWhip>());
                //Make sure its not the same as type1
                while (type4 == type1)
                {
                    type4 = Utils.SelectRandom(WorldGen.genRand,
                    ModContent.ItemType<Florapedia>(), ModContent.ItemType<BlossomingGale>(),
                    ModContent.ItemType<ArsenalRepeater>(), ModContent.ItemType<Bloomhammer>(), ModContent.ItemType<ArsenalWhip>());
                }
                chestItemList.Insert(0, new ChestItem(type4, 1)); break;
            // Terrarium room
            case 2:
                chestItemList.RemoveAt(0); // No weapon here
                chestItemList.Insert(0, new ChestItem(ItemID.DontHurtCrittersBook, 1)); break;
            // Case 3 is Evil room
            case 3:
                chestItemList.RemoveAt(0); // No weapon here
                // Replace with the schematic for this lab
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<EncryptedSchematicPlanetoid>(), 1));
                chestItemList.Insert(1, new ChestItem(ModContent.ItemType<ApertureCore>(), 1)); break;
            // Right tall room with planter boxes
            case 4:
                chestItemList.RemoveAt(0); // No weapon here
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<SuspiciousScrap>(), 1));
                chestItemList.Insert(1, new ChestItem(ItemID.HerbBag, WorldGen.genRand.Next(5, 10))); break;
            // Herb room
            case 5:
                // Replace with log for this lab
                chestItemList.RemoveAt(0); // No weapon here
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<DraedonsLogPlanetoid>(), 1));
                chestItemList.Insert(1, new ChestItem(ItemID.HerbBag, WorldGen.genRand.Next(5, 10))); break;
            // Right Side after trial 1
            case 6:
                chestItemList.Insert(7, new ChestItem(ModContent.ItemType<SpelunkerChip>(), 1)); break;
        }
        ImportantItemID++;

        // Adds a 50% chance for a bonus item, and then rolls for what item will it be, checks if the chest already has one of em guaranteed
        // I need to check if adding these is ok
        float rng = WorldGen.genRand.NextFloat();
        if (rng >= 0.5f && rng < 0.75f && ImportantItemID != 3)
            chestItemList.Insert(7, new ChestItem(ModContent.ItemType<ApertureCore>(), 1));
        else if (rng >= 0.75f && rng < 0.9f && ImportantItemID != 6)
            chestItemList.Insert(7, new ChestItem(ModContent.ItemType<SpelunkerChip>(), 1));
        else if (rng >= 0.5f && rng < 0.9f)
            // Add a protoportal gun if it already has the guaranteed item and if < 0.9, mostly so that its not just one per world so multiplayer can get more
            chestItemList.Insert(7, new ChestItem(ModContent.ItemType<ProtoPortalGun>(), 1));
        else if (rng >= 0.9f)
        {
            // Bonus chance for Dev Items (VANITY ONLY) at 10% chance, feel free to add an item you want! :D
            // Current dev's items:
            // Heart: Hapu Fruit
            // ClockworkShade: Steampunk Goggles (placeholder)
            int devItem = Utils.SelectRandom(WorldGen.genRand,
                    ModContent.ItemType<HapuFruit>(), ItemID.SteampunkGoggles);
            chestItemList.Insert(1, new ChestItem(devItem, 1));
        }
        // Add Juggernuts if on Gfb at a 33% chance
        if (Main.zenithWorld && rng < 0.33f)
            chestItemList.Insert(7, new ChestItem(ModContent.ItemType<Juggernut>(), WorldGen.genRand.Next(1,3)));
        // The whole lab is a Portal Reference, this has to be in
        else if (Main.remixWorld && rng >= 0.33f && rng <= 0.5f)
            chestItemList.Insert(7, new ChestItem(ModContent.ItemType<LemonNade>(), 1));
        for (int index = 0; index < chestItemList.Count; ++index)
        {
            chest.item[index].SetDefaults(chestItemList[index].Type);
            chest.item[index].stack = chestItemList[index].Stack;
        }
    }

    // Having the stuff you're exploring the lab for in the first place generate here is unideal so here is a nerfed version of the loot
    public static void FillTopChests(Chest chest, int type, bool seekingMechanism)
    {
        // Random flashlight lens
        int type1 = Utils.SelectRandom(WorldGen.genRand,
            ModContent.ItemType<AmberFlashlightLens>(), ModContent.ItemType<AmethystFlashlightLens>(), ModContent.ItemType<DiamondFlashlightLens>(),
            ModContent.ItemType<EmeraldFlashlightLens>(), ModContent.ItemType<RubyFlashlightLens>(),
            ModContent.ItemType<SapphireFlashlightLens>(), ModContent.ItemType<TopazFlashlightLens>());
        // Random Ingot Bar
        int type2 = Utils.SelectRandom(WorldGen.genRand,
            ItemID.IronBar, ItemID.LeadBar,
            ItemID.CopperBar, ItemID.TinBar);
        // Random Potion
        int type3 = Utils.SelectRandom(WorldGen.genRand, ItemID.LesserHealingPotion, ItemID.LesserManaPotion, ItemID.ShinePotion, ItemID.SwiftnessPotion, ItemID.IronskinPotion, ItemID.RegenerationPotion);
        List<ChestItem> chestItemList = new List<ChestItem>()
          {
            new ChestItem(type1, 1),
            new ChestItem(type2, WorldGen.genRand.Next(4, 7)),
            new ChestItem(ItemID.Torch, WorldGen.genRand.Next(2, 5)),
            new ChestItem(type3, WorldGen.genRand.Next(1, 4)),
            new ChestItem(ItemID.SilverCoin, WorldGen.genRand.Next(15, 30))
          };
        if (!seekingMechanism)
        {
            float rng = WorldGen.genRand.NextFloat();
            // Adds a Lab Seeking Mechanism at a 50% chance, or any of the 5 Bio Lab Seeking Mechanisms at a 10% chance each
            // Same rates as Calamity has for its own labs
            if (rng < 0.5f)
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<LabSeekingMechanism>(), 1));
            else if (rng < 0.6f)
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<CyanSeekingMechanism>(), 1));
            else if (rng < 0.7f)
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<GreenSeekingMechanism>(), 1));
            else if (rng < 0.8f)
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<WhiteSeekingMechanism>(), 1));
            else if (rng < 0.9f)
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<RedSeekingMechanism>(), 1));
            else
                chestItemList.Insert(0, new ChestItem(ModContent.ItemType<YellowSeekingMechanism>(), 1));
        }

        for (int index = 0; index < chestItemList.Count; ++index)
        {
            chest.item[index].SetDefaults(chestItemList[index].Type);
            chest.item[index].stack = chestItemList[index].Stack;
        }
    }
    #endregion
}
