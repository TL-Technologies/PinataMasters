namespace Modules.Legacy.TextureManagement.Batching
{
	public enum tmBatchingType
	{
		None      = 0,
		Dynamic   = 1 << 0,
		Static    = 1 << 2,
		Skinning  = 1 << 3
	}
}