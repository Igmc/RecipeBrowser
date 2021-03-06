﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace RecipeBrowser.TagHandlers
{
	public class NPCTagHandler : ITagHandler
	{
		private class NPCSnippet : TextSnippet
		{
			private int npcType;
			private bool head;

			public NPCSnippet(int npctype, bool head = false)
				: base("")
			{
				this.npcType = npctype;
				this.head = head;
				//this.Color = ItemRarity.GetColor(item.rare);
			}

			public override void OnHover()
			{
				if (npcType >= Terraria.ID.NPCID.Count)
				{
					ModNPC modNPC = NPCLoader.GetNPC(npcType);
					Main.hoverItemName = Lang.GetNPCNameValue(npcType) + (modNPC != null ? " [" + modNPC.mod.Name + "]" : "");
				}
				else
				{
					Main.hoverItemName = Lang.GetNPCNameValue(npcType);
				}
				//Main.HoverItem = this._item.Clone();
				//Main.instance.MouseText(this._item.Name, this._item.rare, 0, -1, -1, -1, -1);
			}

			public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default(Vector2), Color color = default(Color), float scale = 1f)
			{
				//float drawScale = 1f;
				//float verticalScaleToFit = 1f;
				float maxHeight = 24 * scale;
				Texture2D texture2D = null;
				Rectangle rectangle = new Rectangle();
				if (Main.netMode != 2 && !Main.dedServ)
				{
					Main.instance.LoadNPC(npcType);
					texture2D = Main.npcTexture[npcType];
					rectangle = new Rectangle(0, (Main.npcTexture[npcType].Height / Main.npcFrameCount[npcType]) * 0, Main.npcTexture[npcType].Width, Main.npcTexture[npcType].Height / Main.npcFrameCount[npcType]);

					if (head)
					{
						int headIndex = NPC.TypeToHeadIndex(npcType);
						if(headIndex != -1)
						{
							texture2D = Main.npcHeadTexture[headIndex];
							rectangle = texture2D.Bounds; // or texture2D.Frame(1, 1, 0, 0);
						}
					}

					//if (rectangle.Height > maxHeight)
					//{
					//	verticalScaleToFit = maxHeight / (float)rectangle.Height;
					//}
					if (rectangle.Width * scale > maxHeight || rectangle.Height * scale > maxHeight)
					{
						if (rectangle.Width > rectangle.Height)
						{
							scale = maxHeight / rectangle.Width;
						}
						else
						{
							scale = maxHeight / rectangle.Height;
						}
					}
				}
				//verticalScaleToFit *= scale;
				//drawScale *= verticalScaleToFit;
				//if (drawScale > 0.75f)
				//{
				//	drawScale = 0.75f;
				//}
				if (!justCheckingString && color != Color.Black)
				{
					//float inventoryScale = Main.inventoryScale;
					//Main.inventoryScale = scale * num;
					//if (Main.rand.NextBool())
					Main.spriteBatch.Draw(texture2D, position + new Vector2(maxHeight/2)/*- new Vector2(10f) * scale * num*/, rectangle, /*color*/ Color.White, 0, rectangle.Center(), scale, SpriteEffects.None, 0);
					//else
					//	Main.spriteBatch.Draw(texture2D, position  /*- new Vector2(10f) * scale * num*/, rectangle, color, 0, Vector2.Zero, num, SpriteEffects.None, 0);
					//ItemSlot.Draw(spriteBatch, ref this._item, 14, position - new Vector2(10f) * scale * num, Color.White);
					//int s =24;
					//Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X, (int)position.Y, s, s), Color.Red * 0.3f);
					//Main.inventoryScale = inventoryScale;
				}
				//size = new Vector2(maxHeight) * scale /** drawScale*/;
				size = rectangle.Size() * scale + new Vector2(2, 0);
				return true;
			}

			public override float GetStringLength(DynamicSpriteFont font)
			{
				float h = font.MeasureString("TTHW").X;
				return 32f * this.Scale * 0.65f;
			}
		}

		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
		{
			int npcid;
			if (!int.TryParse(text, out npcid) || npcid >= NPCLoader.NPCCount || npcid <= 0)
			{
				return new TextSnippet(text);
			}
			bool head = false;
			if(options != null)
			{
				//string[] optionArray = options.Split(',');
				if (options == "head")
					head = true;
			}
			return new NPCTagHandler.NPCSnippet(npcid, head)
			{
				Text = GenerateTag(npcid),
				CheckForHover = true,
				DeleteWhole = true
			};
		}

		public static string GenerateTag(int npcID)
		{
			return $"[npc:{npcID}]";
		}
	}
}
