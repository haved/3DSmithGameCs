using System.IO;
using OpenTK;

namespace DSmithGameCs
{
	public class CastItem : Item
	{
		protected static readonly Matrix4 GItemMatrix = Matrix4.CreateScale (1 / 1.5f) * Matrix4.CreateRotationZ (Util.PI / 2) * Matrix4.CreateRotationX (0.1f) * Matrix4.CreateRotationY (-0.1f) *
			Matrix4.CreateTranslation (0, 0, -2.8f) * Matrix4.CreatePerspectiveFieldOfView (0.85f, 1, 0.1f, 4);//*Matrix4.CreateScale(1, 0.5f, 1);

		public static CastItemInfo IngotCast = new CastItemInfo (0, MeshCollection.IngotCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1), "Ingot", 1, 1, 3, 0.63f, new IngotItemCreator());
		public static CastItemInfo GreatsowordCast = new CastItemInfo (1, MeshCollection.GreatswordCast, new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1), "Greatsword", 2, BladeItem.GreatswordBlade.Volume, 6, .4f, new BladeCreator(BladeItem.GreatswordBlade));

		public class CastItemInfo
		{
			public static CastItemInfo[] CastItemsInfo = new CastItemInfo[20];

			public readonly byte Id;
			public readonly Mesh Mesh;
			public readonly Vector4 Color;
			public readonly string Name;
			public readonly uint Size;
			public readonly float Volume;
			public readonly float Width;
			public readonly float Height;
			public readonly ICastItemCreator ItemCreator;
			public readonly Matrix4 ItemMatrix;

			public CastItemInfo (byte id, Mesh mesh, Vector4 color, string name, uint size, float volume, float width, float height, ICastItemCreator itemCreator)
			{
				Id = id;
				Mesh = mesh;
				Color = color;
				Name = name;
				Size = size;
				Volume = volume;
				Width = width;
				Height = height;
				ItemCreator = itemCreator;
				ItemMatrix = (width > 6) ? Matrix4.CreateScale(6/width, 6/width, 1) : Matrix4.Identity;
				ItemMatrix *= GItemMatrix * Matrix4.CreateScale(1, 1f/size, 1);
				CastItemsInfo[id] = this;
			}
		}

		public byte CastInfoID;
		public CastItemInfo Info { private set; get; }

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
			Info = CastItemInfo.CastItemsInfo[castInfoID];
		}

		public override void RenderItem(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (Info.Mesh, Info.ItemMatrix, Info.Color, x+4, y+4, width-8, height-8);
		}

		public Mesh GetMesh()
		{
			return Info.Mesh;
		}

		public Vector4 GetColor()
		{
			return Info.Color;
		}

		public override uint GetSize()
		{
			return Info.Size;
		}

		public string GetTooltipName()
		{
			return Localization.GetLocalization("ui.cast." + Info.Name) + Localization.GetLocalization("ui.item.cast");
		}

		public override void DrawTooltip (TextWriter writer)
		{
			writer.DrawStandardTooltip (new []{ GetTooltipName (), Localization.GetLocalization ("ui.tooltip.volume:") }, 
				new []{ Util.GetColorFromVector (Info.Color) }, new []{null, Info.Volume + " " + Localization.GetLocalization("ui.item.ingot")});
		}

		public float GetVolume()
		{
			return Info.Volume;
		}

		public float FillHeight
		{
			get { return Info.Height; }
		}

		public Item CreateItem(Smith2DGame game, int metal, float purity)
		{
			if (Info.ItemCreator != null && Info.ItemCreator.CanCreateItem (metal, purity, game))
				return Info.ItemCreator.CreateItem (metal, purity);
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

