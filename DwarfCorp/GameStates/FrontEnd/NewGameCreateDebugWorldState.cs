using System;
using System.Collections.Generic;
using DwarfCorp.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DwarfCorp.GameStates
{
    public class NewGameCreateDebugWorldState : MenuState
    {
        public NewGameCreateDebugWorldState(DwarfGame game, GameStateManager stateManager) :
            base("MainMenuState", game, stateManager)
        {
        }

        public void MakeDebugWorldMenu()
        {
            var frame = CreateMenu("SPECIAL WORLDS");

            CreateMenuItem(frame, "Hills", "Create a hilly world.", (sender, args) =>
                {
                    Overworld.CreateHillsLand(Game.GraphicsDevice);
                    StateManager.ClearState();
                    WorldGenerationSettings settings = new WorldGenerationSettings()
                    {
                        ExistingFile = null,
                        ColonySize = new Point3(8, 1, 8),
                        WorldScale =  2.0f,
                        WorldOrigin = new Vector2(Overworld.Map.GetLength(0)/2.0f,
                            Overworld.Map.GetLength(1)/2.0f)*0.5f,
                        SpawnRect = new Rectangle((int)(Overworld.Map.GetLength(0) / 2.0f - 8 * VoxelConstants.ChunkSizeX),
                            (int)(Overworld.Map.GetLength(1) / 2.0f - 8 * VoxelConstants.ChunkSizeX),
                            8 * VoxelConstants.ChunkSizeX, 8 * VoxelConstants.ChunkSizeX)
                    };
                    StateManager.PushState(new LoadState(Game, StateManager, settings));
                });

            CreateMenuItem(frame, "Cliffs", "Create a cliff-y world.", (sender, args) =>
                {
                    Overworld.CreateCliffsLand(Game.GraphicsDevice);
                    StateManager.ClearState();
                    WorldGenerationSettings settings = new WorldGenerationSettings()
                    {
                        ExistingFile = null,
                        ColonySize = new Point3(8, 1, 8),
                        WorldScale = 2.0f,
                        WorldOrigin = new Vector2(Overworld.Map.GetLength(0) / 2.0f,
                            Overworld.Map.GetLength(1) / 2.0f) * 0.5f,
                        SpawnRect = new Rectangle((int)(Overworld.Map.GetLength(0) / 2.0f - 8 * VoxelConstants.ChunkSizeX),
                            (int)(Overworld.Map.GetLength(1) / 2.0f - 8 * VoxelConstants.ChunkSizeX),
                            8 * VoxelConstants.ChunkSizeX, 8 * VoxelConstants.ChunkSizeX)
                    };
                    StateManager.PushState(new LoadState(Game, StateManager, settings));
                });

            CreateMenuItem(frame, "Flat", "Create a flat world.", (sender, args) =>
                {
                    Overworld.CreateUniformLand(Game.GraphicsDevice);
                    StateManager.ClearState();
                    WorldGenerationSettings settings = new WorldGenerationSettings()
                    {
                        ExistingFile = null,
                        ColonySize = new Point3(8, 1, 8),
                        WorldScale = 2.0f,
                        WorldOrigin = new Vector2(Overworld.Map.GetLength(0) / 2.0f,
                            Overworld.Map.GetLength(1) / 2.0f) * 0.5f,
                        SpawnRect = new Rectangle((int)(Overworld.Map.GetLength(0) / 2.0f - 8 * VoxelConstants.ChunkSizeX),
                            (int)(Overworld.Map.GetLength(1) / 2.0f - 8 * VoxelConstants.ChunkSizeX),
                            8 * VoxelConstants.ChunkSizeX, 8 * VoxelConstants.ChunkSizeX)
                    };
                    StateManager.PushState(new LoadState(Game, StateManager, settings));
                });

            CreateMenuItem(frame, "Ocean", "Create an ocean world", (sender, args) =>
                {
                    Overworld.CreateOceanLand(Game.GraphicsDevice, 0.17f);
                    StateManager.ClearState();
                    WorldGenerationSettings settings = new WorldGenerationSettings()
                    {
                        ExistingFile = null,
                        ColonySize = new Point3(8, 1, 8),
                        WorldScale = 2.0f,
                        WorldOrigin = new Vector2(Overworld.Map.GetLength(0) / 2.0f,
                            Overworld.Map.GetLength(1) / 2.0f) * 0.5f,
                        SpawnRect = new Rectangle((int)(Overworld.Map.GetLength(0) / 2.0f - 8 * VoxelConstants.ChunkSizeX),
                            (int)(Overworld.Map.GetLength(1) / 2.0f - 8 * VoxelConstants.ChunkSizeX),
                            8 * VoxelConstants.ChunkSizeX, 8 * VoxelConstants.ChunkSizeX)
                    };
                    StateManager.PushState(new LoadState(Game, StateManager, settings));
                });

            CreateMenuItem(frame, "Back", "Go back to the main menu.", (sender, args) => StateManager.PopState());

            FinishMenu();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            MakeDebugWorldMenu();
            IsInitialized = true;
        }

        public override void Update(DwarfTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Render(DwarfTime gameTime)
        {
            base.Render(gameTime);
        }
    }

}