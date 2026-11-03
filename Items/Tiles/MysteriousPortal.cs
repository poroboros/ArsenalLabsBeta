using ArsenalLabsBeta.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArsenalLabsBeta.Items.Tiles
{
    public class MysteriousPortal : ModTile
    {
        //entirely recycled from the   shield generator cause i'm Lazy, plus the tile action code will be nice to have later

        public int width = 11;
        public int height = 8;
        public int yOffset = 5;

        public override void SetStaticDefaults()
        {
            MinPick = 1000;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
            TileObjectData.newTile.Width = 11;
            TileObjectData.newTile.Height = 8;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16 };

            //AnimationFrameHeight = 72;
            TileObjectData.addTile(Type);
            AddMapEntry(ArsenalLabsHelpers.FurnitureColor, CreateMapEntryName());
        }

        /*public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter > 6)
			{
				frameCounter = 0;
				frame++;
				if (frame >= 8)
				{
					frame = 0;
				}
			}
		}

		public override bool Drop(int i, int j)
        {
            return base.Drop(i, j);
        }
		*/
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            ArsenalLabsTile.QuickGlowmask(this, spriteBatch, i, j);
            /*if (ArsenalLabsBeta.GameWorldActive)
			{
				ILoveTileActions(i, j);
			}*/
        }
        /*
		public override bool RightClick(int i, int j)
		{
			active = !active;
			return true;
		}

		public void ILoveTileActions(int i, int j)
		{
			if (!active)
				return;
			if (PlayersToDoEffects == null)
			{
				PlayersToDoEffects = new List<int>();
			}
			if (CheckPlayerList <= 0)
			{
				PlayersToDoEffects.Clear();
				var r = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
				for (int pp = 0; pp < Main.maxPlayers; pp++)
				{
					if (Main.player[pp].active && !Main.player[pp].dead)
					{
						PlayersToDoEffects.Add(pp);
					}
				}
				CheckPlayerList += 70 - Main.fpsCount;
			}

			var tileWorld = new Vector2(i * 16f + 8f, j * 16f + 8f);

			Vector2[] points = new Vector2[]
			{
				new Vector2(tileWorld.X - width*8, tileWorld.Y - (height)*16),
				new Vector2(tileWorld.X - width*8, tileWorld.Y +(yOffset)*16),
				new Vector2(tileWorld.X + width*8, tileWorld.Y - (height)*16),
				new Vector2(tileWorld.X + width*8, tileWorld.Y + (yOffset)*16),
			};

			//CalamityUtils.DrawLineBetter(Main.spriteBatch, points[0], points[1], Color.Teal, 2);
			//CalamityUtils.DrawLineBetter(Main.spriteBatch, points[0], points[3], Color.Teal, 2);
			//CalamityUtils.DrawLineBetter(Main.spriteBatch, points[1], points[3], Color.Teal, 2);
			//CalamityUtils.DrawLineBetter(Main.spriteBatch, points[3], points[3], Color.Teal, 2);

			//foreach (Vector2 p in points)
			//    Dust.NewDust(p, 1, 1, DustID.RedTorch, 0, 0);


			foreach (var p in PlayersToDoEffects)
			{
				if (Main.player[p].active && !Main.player[p].dead && !Main.player[p].Catalyst().resistMetanovaGravity)
				{
					Rectangle rect = new Rectangle(((int)tileWorld.X) - width * 8, ((int)tileWorld.Y) - (yOffset) * 16, width * 8, (height) * 16);
					var d = Main.player[p].Distance(tileWorld);
					if (Main.player[p].getRect().Intersects(rect))
					{
						Main.player[p].AddBuff(ModContent.BuffType<SuppressionField>(), 20);
					}
				}
			}

		}*/
    }
}
