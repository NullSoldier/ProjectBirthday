using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Objects;
using ProjectB.States;

namespace ProjectB
{
	public class BaseLevel
	{
		public BaseLevel (string levelTexture)
		{
			CollisionGeometry = new List<CollisionGeometry> ();
			GameObjects = new List<GameObject>();
			Ladders = new List<Ladder>();
			Chests = new List<Chest>();

			Level = new GameObject
			{
				Texture = Engine.ContentManager.Load<Texture2D> (levelTexture)
			};
		}

		public void SpawnPlayer (Vector2 location)
		{
			Player = new Player
			{
				Location = location
			};
			Player.Reset();
		}

		public PatrolEnemy SpawnWalker (Vector2 location, Directions direction, float maxLeft, float maxRight)
		{
			var newWalker = new PatrolEnemy (location, direction, maxLeft, maxRight);
			GameObjects.Add (newWalker);

			return newWalker;
		}

		public SentryEnemy SpawnSentry (Vector2 location, Directions direction, float fireSpeed)
		{
			var newSentry = new SentryEnemy (location, direction, fireSpeed);
			GameObjects.Add (newSentry);

			return newSentry;
		}

		public bool DisplayInterface;
		public GameObject Level;
		public Player Player;
		public int FriendCount;
		public Vector2 StartPoint;
		public CaveExit Exit;
		public Color SkyColor;
		public bool UseClouds;
		public List<CollisionGeometry> CollisionGeometry;
		public List<Ladder> Ladders;
		public List<Chest> Chests;
		public List<GameObject> GameObjects;

		public virtual void Update (GameTime gameTime)
		{
		}

		public virtual void Start (GameState gameState)
		{
		}

		protected void AddGeometry (Rectangle rect, CollisionType collisionType)
		{
			CollisionGeometry.Add (new CollisionGeometry(rect, collisionType));
		}

		protected void AddLadder (Rectangle rect)
		{
			Ladders.Add (new Ladder(rect));
		}

		protected void AddChest (Vector2 position, Friend friend)
		{
			Chest chest = new Chest { Friend = friend, Location = position, Opened = false };
			Chests.Add (chest);
			GameObjects.Add (chest);
		}
	}
}
