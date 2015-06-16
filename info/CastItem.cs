using System.IO;
using OpenTK;

namespace DSmithGameCs
{
	public class CastItem : Item
	{
		public static CastItemInfo IngotCast = new CastItemInfo (0, MeshCollection.IngotCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1), "Ingot", 1, 0.63f, new IngotItemCreator());
		public static CastItemInfo GreatsowordCast = new CastItemInfo (1, MeshCollection.GreatswordCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1), "Greatsword", BladeItem.GreatswordBlade.Volume, 0.4f, new BladeCreator(BladeItem.GreatswordBlade));

		public struct CastItemInfo
		{
			public static CastItemInfo[] CastItemsInfo = new CastItemInfo[20];

			public readonly byte Id;
			public readonly Mesh Mesh;
			public readonly Vector4 Color;
			public readonly string Name;
			public readonly float Volume;
			public readonly float Height;
			public readonly ICastItemCreator ItemCreator;

			public CastItemInfo (byte id, Mesh mesh, Vector4 color, string name, float volume, float height, ICastItemCreator itemCreator)
			{
				Id = id;
				Mesh = mesh;
				Color = color;
				Name = name;
				Volume = volume;
				Height = height;
				ItemCreator = itemCreator;
				CastItemsInfo[id] = this;
			}
		}

		public byte CastInfoID;
		CastItemInfo info;

		public CastItem(){}

		public CastItem(CastItemInfo info)
		{
			SetCastInfoID (info.Id);
		}

		public CastItem (byte castInfoID)
		{
			SetCastInfoID (castInfoID);
		}

		void SetCastInfoID(byte castInfoID)
		{
			CastInfoID = castInfoID;
			info = CastItemInfo.CastItemsInfo[castInfoID];
		}

		protected static readonly Matrix4 ItemMatrix = Matrix4.CreateRotationZ(Util.PI/2)*Matrix4.CreateRotationX(0.1f)*Matrix4.CreateRotationY(-0.1f)*Matrix4.CreateTranslation(0, 0, -2.8f)
			*Matrix4.CreatePerspectiveFieldOfView(0.85f, 1, 0.1f, 4);
		public override void RenderItem(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (info.Mesh, ItemMatrix, info.Color, x+4, y+4, width-8, height-8, -1, -2, 2, 4);
		}

		public Mesh GetMesh()
		{
			return info.Mesh;
		}

		public Vector4 GetColor()
		{
			return info.Color;
		}

		public override uint GetSize()
		{
			return 2;
		}

		public string GetTooltipName()
		{
			return Localization.GetLocalization("ui.cast." + info.Name) + Localization.GetLocalization("ui.item.cast");
		}

		public override void DrawTooltip (TextWriter writer)
		{
			DrawStandardTooltip (writer, new []{ GetTooltipName (), Localization.GetLocalization ("ui.tooltip.volume:") }, 
				new []{ Util.GetColorFromVector (info.Color) }, new []{null, info.Volume + " " + Localization.GetLocalization("ui.item.ingot")});
		}

		public float GetVolume()
		{
			return info.Volume;
		}

		public float FillHeight
		{
			get { return info.Height; }
		}

		public Item CreateItem(Smith2DGame game, int metal, float purity)
		{
			if (info.ItemCreator != null && info.ItemCreator.CanCreateItem (metal, purity, game))
				return info.ItemCreator.CreateItem (metal, purity);
			return null;
		}

		public override void LoadInfoFromFile(Stream reader)
		{
			SetCastInfoID((byte)reader.ReadByte());
		}

		public override void SaveInfoToFile(Stream writer)
		{
			writer.WriteByte (CastInfoID);
		}
	}
}

