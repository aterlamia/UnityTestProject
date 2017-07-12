using System.Collections;
using System.Collections.Generic;

public class Tile {
	private TileType _tileType;	
	private bool _mineable = true;
	private int _x;
	private int _y;
	public Tile(TileType type, int x, int y) {
		_tileType = type;
		_x = x;
		_y = y;
	}

	public TileType getType() {
		return _tileType;
	}

	public void setMineable(bool mineable) {
		_mineable = mineable;
	}

	public bool isMineable() {
		return _mineable;
	}
}
