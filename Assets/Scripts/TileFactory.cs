using System;

public enum TileType {Empty, Ground, HardGround, Sand, Rock, Mud, Water, Clay};  

public class TileFactory
{
	public Tile createTile(TileType type, int x, int y) {
		Tile tile = new Tile (type, x, y);

		return tile;
	}

	private void setMinable(Tile tile) {
		if (tile.getType () == TileType.Empty || tile.getType () == TileType.Rock || tile.getType () == TileType.Water) {
			tile.setMineable (false);
		}
	}		
}
