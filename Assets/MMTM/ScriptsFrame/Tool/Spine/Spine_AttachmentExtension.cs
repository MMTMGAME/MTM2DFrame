using Spine;
using UnityEngine;
using Spine.Unity.AttachmentTools;



//TODO:这个替换图片缩放会变得和这个插槽上的一样的大小,解决办法在spine中制作各种不同大小的皮肤，然后武器图片必须都是正方形
//TODO:Material这个需要定时清理,不然会一直占用内存
//bug:使用以后切换场景被替换的那个插槽有问题
/// <summary>
/// 动态替换图片
/// </summary>
public static class Spine_AttachmentExtension 
{
	/// <summary>
	/// 替换MeshAttachment中的图片,也就是刷过网格的图片
	/// 如果连续使用的话需要DestroyImmediate(m.mainTexture,false);
	/// </summary>
	/// <param name="slot">SlotName</param>
	/// <param name="sprite"></param>
	/// <returns>之前的Material</returns>
	public static Material ChangeMeshAttachment(this Slot slot,Sprite sprite)
	{
		if(slot==null || sprite == null) return null;
		MeshAttachment oldAtt = slot.Attachment as MeshAttachment;
		if(oldAtt==null) return null;

		MeshAttachment att = new MeshAttachment(oldAtt.Name);
		
		att.Region = sprite.texture.CreateAtlasRegion();
		att.Path = oldAtt.Path;
 
		att.Bones = oldAtt.Bones;
		att.Edges = oldAtt.Edges;
		att.Triangles = oldAtt.Triangles;
		att.Vertices = oldAtt.Vertices;
		att.WorldVerticesLength = oldAtt.WorldVerticesLength;
		att.HullLength = oldAtt.HullLength;
		//att.RegionRotate = false;

		att.Region.u = 0;
		att.Region.v = 1;
		att.Region.u2 = 1;
		att.Region.v2 = 0;
		
		att.RegionUVs = oldAtt.RegionUVs;//所以武器图片的网格应该全张图片
		
		att.UpdateRegion();
 
		Material mat = new Material(Shader.Find("Sprites/Default"));
		mat.mainTexture = sprite.texture;
		(att.Region as AtlasRegion).page.rendererObject = mat;
 
		slot.Attachment = att;
		return mat;
	}
	public static AtlasRegion CreateAtlasRegion(this Texture2D texture){
 
		AtlasRegion region = new AtlasRegion();
		region.width = texture.width;
		region.height = texture.height;
		region.originalWidth = texture.width;
		region.originalHeight = texture.height;
		region.rotate = false;
		region.page = new AtlasPage();
		region.page.name = texture.name;
		region.page.width = texture.width;
		region.page.height = texture.height;
		region.page.uWrap = TextureWrap.ClampToEdge;
		region.page.vWrap = TextureWrap.ClampToEdge;
 
		return region;
	}
	
	
	/// <summary>
	/// 替换RegionAttachment，也就是没有刷网格的图片
	/// </summary>
	/// <param name="slot"></param>
	/// <param name="sprite"></param>
	/// <returns></returns>
	public static Material CreateRegionAttachmentByTexture(this Slot slot, Sprite sprite)
	{
		if(slot==null || sprite) return null;
		RegionAttachment oldAtt = slot.Attachment as RegionAttachment;
		if(oldAtt==null) return null;
		RegionAttachment att = new RegionAttachment(oldAtt);
		att.Region = sprite.texture.CreateAtlasRegion();
		att.Width = oldAtt.Width;
		att.Height = oldAtt.Height;
		att.SetPositionOffset(Vector2.zero);
		att.Path = oldAtt.Path;
	
		att.X = oldAtt.X;
		att.Y = oldAtt.Y;
		att.Rotation = oldAtt.Rotation;
		att.ScaleX = oldAtt.ScaleX;
		att.ScaleY = oldAtt.ScaleY;
	
		att.Region.u = 0;
		att.Region.v = 1;
		att.Region.u2 = 1;
		att.Region.v2 = 0;
		//att.SetUVs(0f,1f,1f,0f,false);
 
		Material mat = new Material(Shader.Find("Sprites/Default"));
		mat.mainTexture = sprite.texture;
		(att.Region as Spine.AtlasRegion).page.rendererObject = mat;
		slot.Attachment = att;

		return mat;
	}
}
