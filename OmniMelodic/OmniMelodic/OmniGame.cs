//#define LOOPINGVECTORS

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace OmniMelodic
{
    //ORDER IS RELEVANT
    enum Direction { N, NE, E, SE, S, SW, W, NW };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class OmniGame : Microsoft.Xna.Framework.Game
    {
        //const double TAN_15 = 0.267949192431123;
        //const double TAN_75 = 3.732050807568878;

        //every direction gets 45deg (22.5deg +/-)
        const double TAN_22_5 = 0.414213562373095;
        const double TAN_67_5 = 2.414213562373094;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PrimitiveBatch primitiveBatch;

        Texture2D texMenuBar, texDelete, texPix, texCircle, texTrash,
                  c_low_on, c_low_off, c_mid_on, c_mid_off, c_high_on, c_high_off,
                  e_low_on, e_low_off, e_mid_on, e_mid_off, e_high_on, e_high_off,
                  g_low_on, g_low_off, g_mid_on, g_mid_off, g_high_on, g_high_off,
                  b_low_on, b_low_off, b_mid_on, b_mid_off, b_high_on, b_high_off;

        MouseState currentMouseState, prevMouseState;
        KeyboardState prevKeyState;
        bool bDragging, bMenu, bResize, bConnecting;

        Direction currDirection;
        double dragRatio;

        static Grid grid;

        //currently selected note
        OmniNote selectNote;
        Tuple<int, int> selectCell;

        Rectangle boundDel, boundTrash;
        BoundingSphere boundRedLt, boundRedMd, boundRedDk,
                       boundOrangeLt, boundOrangeMd, boundOrangeDk, 
                       boundGreenLt, boundGreenMd, boundGreenDk,
                       boundTealLt, boundTealMd, boundTealDk,
                       boundResize;

        //Sound files
        static SoundEffect[][] bNote, cNote, eNote, gNote;

        static List<Vector> vectors;

        static Dictionary<OmniNote, List<OmniNote>> noteGroups;

        public OmniGame()
        {
            graphics = new GraphicsDeviceManager(this);

            //default window size
            graphics.PreferredBackBufferWidth = 701;
            graphics.PreferredBackBufferHeight = 701;
            
            graphics.IsFullScreen = false;

            System.Windows.Forms.Form MyGameForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            MyGameForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;

            //show mouse
            IsMouseVisible = true;

            bDragging = false;
            bMenu = false;
            bResize = false;

            Content.RootDirectory = "Content";

            vectors = new List<Vector>();
            noteGroups = new Dictionary<OmniNote, List<OmniNote>>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            grid = new Grid();

            base.Initialize();
        }


        void UpdateSprite(GameTime gameTime)
        {
            /*
            // Move the sprite by speed, scaled by elapsed time.
            spritePosition +=
                spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            */
        }

        protected override void LoadContent()
        {
            bNote = new SoundEffect[3][];
            eNote = new SoundEffect[3][];
            cNote = new SoundEffect[3][];
            gNote = new SoundEffect[3][];
            for (int n = 0; n < 3; n++)
            {
                bNote[n] = new SoundEffect[5];
                cNote[n] = new SoundEffect[5];
                eNote[n] = new SoundEffect[5];
                gNote[n] = new SoundEffect[5];
            }

            bNote[0][0] = Content.Load<SoundEffect>("Cello_B_Low_02");
            bNote[0][1] = Content.Load<SoundEffect>("Cello_B_Low_04");
            bNote[0][2] = Content.Load<SoundEffect>("Cello_B_Low_06");
            bNote[0][3] = Content.Load<SoundEffect>("Cello_B_Low_08");
            bNote[0][4] = Content.Load<SoundEffect>("Cello_B_Low_10");
            bNote[1][0] = Content.Load<SoundEffect>("Cello_B_Mid_02");
            bNote[1][1] = Content.Load<SoundEffect>("Cello_B_Mid_04");
            bNote[1][2] = Content.Load<SoundEffect>("Cello_B_Mid_06");
            bNote[1][3] = Content.Load<SoundEffect>("Cello_B_Mid_08");
            bNote[1][4] = Content.Load<SoundEffect>("Cello_B_Mid_10");
            bNote[2][0] = Content.Load<SoundEffect>("Cello_B_High_02");
            bNote[2][1] = Content.Load<SoundEffect>("Cello_B_High_04");
            bNote[2][2] = Content.Load<SoundEffect>("Cello_B_High_06");
            bNote[2][3] = Content.Load<SoundEffect>("Cello_B_High_08");
            bNote[2][4] = Content.Load<SoundEffect>("Cello_B_High_10");

            cNote[0][0] = Content.Load<SoundEffect>("Cello_C_Low_02");
            cNote[0][1] = Content.Load<SoundEffect>("Cello_C_Low_04");
            cNote[0][2] = Content.Load<SoundEffect>("Cello_C_Low_06");
            cNote[0][3] = Content.Load<SoundEffect>("Cello_C_Low_08");
            cNote[0][4] = Content.Load<SoundEffect>("Cello_C_Low_10");
            cNote[1][0] = Content.Load<SoundEffect>("Cello_C_Mid_02");
            cNote[1][1] = Content.Load<SoundEffect>("Cello_C_Mid_04");
            cNote[1][2] = Content.Load<SoundEffect>("Cello_C_Mid_06");
            cNote[1][3] = Content.Load<SoundEffect>("Cello_C_Mid_08");
            cNote[1][4] = Content.Load<SoundEffect>("Cello_C_Mid_10");
            cNote[2][0] = Content.Load<SoundEffect>("Cello_C_High_02");
            cNote[2][1] = Content.Load<SoundEffect>("Cello_C_High_04");
            cNote[2][2] = Content.Load<SoundEffect>("Cello_C_High_06");
            cNote[2][3] = Content.Load<SoundEffect>("Cello_C_High_08");
            cNote[2][4] = Content.Load<SoundEffect>("Cello_C_High_10");

            eNote[0][0] = Content.Load<SoundEffect>("Cello_E_Low_02");
            eNote[0][1] = Content.Load<SoundEffect>("Cello_E_Low_04");
            eNote[0][2] = Content.Load<SoundEffect>("Cello_E_Low_06");
            eNote[0][3] = Content.Load<SoundEffect>("Cello_E_Low_08");
            eNote[0][4] = Content.Load<SoundEffect>("Cello_E_Low_10");
            eNote[1][0] = Content.Load<SoundEffect>("Cello_E_Mid_02");
            eNote[1][1] = Content.Load<SoundEffect>("Cello_E_Mid_04");
            eNote[1][2] = Content.Load<SoundEffect>("Cello_E_Mid_06");
            eNote[1][3] = Content.Load<SoundEffect>("Cello_E_Mid_08");
            eNote[1][4] = Content.Load<SoundEffect>("Cello_E_Mid_10");
            eNote[2][0] = Content.Load<SoundEffect>("Cello_E_High_02");
            eNote[2][1] = Content.Load<SoundEffect>("Cello_E_High_04");
            eNote[2][2] = Content.Load<SoundEffect>("Cello_E_High_06");
            eNote[2][3] = Content.Load<SoundEffect>("Cello_E_High_08");
            eNote[2][4] = Content.Load<SoundEffect>("Cello_E_High_10");

            gNote[0][0] = Content.Load<SoundEffect>("Cello_G_Low_02");
            gNote[0][1] = Content.Load<SoundEffect>("Cello_G_Low_04");
            gNote[0][2] = Content.Load<SoundEffect>("Cello_G_Low_06");
            gNote[0][3] = Content.Load<SoundEffect>("Cello_G_Low_08");
            gNote[0][4] = Content.Load<SoundEffect>("Cello_G_Low_10");
            gNote[1][0] = Content.Load<SoundEffect>("Cello_G_Mid_02");
            gNote[1][1] = Content.Load<SoundEffect>("Cello_G_Mid_04");
            gNote[1][2] = Content.Load<SoundEffect>("Cello_G_Mid_06");
            gNote[1][3] = Content.Load<SoundEffect>("Cello_G_Mid_08");
            gNote[1][4] = Content.Load<SoundEffect>("Cello_G_Mid_10");
            gNote[2][0] = Content.Load<SoundEffect>("Cello_G_High_02");
            gNote[2][1] = Content.Load<SoundEffect>("Cello_G_High_04");
            gNote[2][2] = Content.Load<SoundEffect>("Cello_G_High_06");
            gNote[2][3] = Content.Load<SoundEffect>("Cello_G_High_08");
            gNote[2][4] = Content.Load<SoundEffect>("Cello_G_High_10");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();

            primitiveBatch = new PrimitiveBatch(graphics.GraphicsDevice);
        }

        private void LoadTextures()
        {
            texMenuBar = Content.Load<Texture2D>("menu");
            texDelete = Content.Load<Texture2D>("x");
            texPix = Content.Load<Texture2D>("pixel");
            texCircle = Content.Load<Texture2D>("circle");
            texTrash = Content.Load<Texture2D>("trash");

            c_low_on = Content.Load<Texture2D>("red_dark_on");
            c_low_off = Content.Load<Texture2D>("red_dark_off");
            c_mid_on = Content.Load<Texture2D>("red_mid_on");
            c_mid_off = Content.Load<Texture2D>("red_mid_off");
            c_high_on = Content.Load<Texture2D>("red_light_on");
            c_high_off = Content.Load<Texture2D>("red_light_off");

            e_low_on = Content.Load<Texture2D>("orange_dark_on");
            e_low_off = Content.Load<Texture2D>("orange_dark_off");
            e_mid_on = Content.Load<Texture2D>("orange_mid_on");
            e_mid_off = Content.Load<Texture2D>("orange_mid_off");
            e_high_on = Content.Load<Texture2D>("orange_light_on");
            e_high_off = Content.Load<Texture2D>("orange_light_off");

            g_low_on = Content.Load<Texture2D>("green_dark_on");
            g_low_off = Content.Load<Texture2D>("green_dark_off");
            g_mid_on = Content.Load<Texture2D>("green_mid_on");
            g_mid_off = Content.Load<Texture2D>("green_mid_off");
            g_high_on = Content.Load<Texture2D>("green_light_on");
            g_high_off = Content.Load<Texture2D>("green_light_off");

            b_low_on = Content.Load<Texture2D>("teal_dark_on");
            b_low_off = Content.Load<Texture2D>("teal_dark_off");
            b_mid_on = Content.Load<Texture2D>("teal_mid_on");
            b_mid_off = Content.Load<Texture2D>("teal_mid_off");
            b_high_on = Content.Load<Texture2D>("teal_light_on");
            b_high_off = Content.Load<Texture2D>("teal_light_off");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if ((Keyboard.GetState().IsKeyUp(Keys.F)) && (prevKeyState.IsKeyDown(Keys.F)))
            {
                System.Windows.Forms.Form MyGameForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
                if (MyGameForm.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None)
                    MyGameForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                else
                    MyGameForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }

            //Update each vector
            int elapsedTimeMs = (int)(gameTime.TotalGameTime.TotalMilliseconds);
            int n = 0;
            while (n < vectors.Count())
            {
                if (vectors.ElementAt(n).UpdatePosition(elapsedTimeMs))
                    n++;
                else
                {
                    foreach (SoundEffectInstance s in vectors.ElementAt(n).effectsToDispose)
                        s.Dispose();
                    vectors.Remove(vectors.ElementAt(n));
                }
            }
            
            if (this.IsActive)
            {
                currentMouseState = Mouse.GetState();

                //left+right PRESSED
                if ((currentMouseState.RightButton == ButtonState.Pressed) && (currentMouseState.LeftButton == ButtonState.Pressed))
                {
                    var cell = GetCellFromCoords(currentMouseState.X, currentMouseState.Y);
                    int i = cell.Item1;
                    int j = cell.Item2;

                    if (grid.IsEmpty(i, j))
                    {
                        if (selectNote == null)
                            selectNote = grid.AddNote(i, j, 0.8, Note.E, Scale.MID);
                        else
                            selectNote = grid.AddNote(i, j, selectNote.Amplitude, selectNote.Tone, selectNote.Octave);
                    }

                    selectCell = new Tuple<int, int>(i, j);
                }
                //left pressed --> RELEASED
                else if ((currentMouseState.LeftButton == ButtonState.Released) && (prevMouseState.LeftButton == ButtonState.Pressed))
                {
                    if (bDragging && selectCell != null)
                    {
                        fireVector(gameTime, false);
                    }
                    else if (bResize)
                        bResize = false;
                }
                //left released --> PRESSED
                else if ((currentMouseState.LeftButton == ButtonState.Pressed) && (prevMouseState.LeftButton == ButtonState.Released))
                {
                    int x = currentMouseState.X;
                    int y = currentMouseState.Y;

                    //check for delete all
                    if (boundTrash.Contains(x, y))
                    {
                        grid.Clear();
                        vectors.Clear();
                        noteGroups.Clear();

                        selectNote = null;
                    }

                    if (bMenu)
                    {
                        //click delete 'x'
                        if (boundDel.Contains(x, y))
                        {
                            //Removes all of the drawing connections

                            int i = selectCell.Item1;
                            int j = selectCell.Item2;

                            if (i > 0)
                            {
                                if (!grid.IsEmpty(i-1,j))
                                    grid.GetNote(i - 1, j).connections[(int)Direction.E] = false;

                                if ((j > 0) && (!grid.IsEmpty(i-1,j-1)))
                                    grid.GetNote(i - 1, j - 1).connections[(int)Direction.SE] = false;

                                if ((j < grid.NumRows - 2) && (!grid.IsEmpty(i - 1, j + 1)))
                                    grid.GetNote(i - 1, j + 1).connections[(int)Direction.NE] = false;
                            }
                            if (i < grid.NumRows - 2)
                            {
                                if (!grid.IsEmpty(i+1,j))
                                    grid.GetNote(i + 1, j).connections[(int)Direction.W] = false;

                                if ((j > 0) && (!grid.IsEmpty(i + 1, j - 1)))
                                    grid.GetNote(i + 1, j - 1).connections[(int)Direction.SW] = false;

                                if ((j < grid.NumRows - 2) && (!grid.IsEmpty(i + 1, j + 1)))
                                    grid.GetNote(i + 1, j + 1).connections[(int)Direction.NW] = false;
                            }
                            if ((j > 0) && (!grid.IsEmpty(i, j - 1)))
                                grid.GetNote(i, j - 1).connections[(int)Direction.S] = false;
                            if ((j < grid.NumRows - 2) && (!grid.IsEmpty(i, j + 1)))
                                grid.GetNote(i, j + 1).connections[(int)Direction.N] = false;

                            List<OmniNote> group;
                            if (noteGroups.TryGetValue(selectNote, out group))
                            {
                                if (group.Count() == 2)
                                {
                                    noteGroups.Remove(group.ElementAt(0));
                                    noteGroups.Remove(group.ElementAt(1));
                                }
                                else
                                {
                                    group.Remove(selectNote);
                                    noteGroups.Remove(selectNote);
                                    List<List<OmniNote>> newGroups = new List<List<OmniNote>>();

                                    if (selectNote.connections[(int)Direction.E] && group.Contains(grid.GetNote(i + 1, j)))
                                        newGroups.Add(regroup(i + 1, j, group));

                                    if (selectNote.connections[(int)Direction.SE] && group.Contains(grid.GetNote(i + 1, j + 1)))
                                        newGroups.Add(regroup(i + 1, j + 1, group));

                                    if (selectNote.connections[(int)Direction.S] && group.Contains(grid.GetNote(i, j + 1)))
                                        newGroups.Add(regroup(i, j + 1, group));
                                    
                                    if (selectNote.connections[(int)Direction.SW] && group.Contains(grid.GetNote(i - 1, j + 1)))
                                        newGroups.Add(regroup(i - 1, j + 1, group));
                                    
                                    if (selectNote.connections[(int)Direction.W] && group.Contains(grid.GetNote(i - 1, j)))
                                        newGroups.Add(regroup(i - 1, j, group));
                                    
                                    if (selectNote.connections[(int)Direction.NW] && group.Contains(grid.GetNote(i - 1, j - 1)))
                                        newGroups.Add(regroup(i - 1, j - 1, group));
                                    
                                    if (selectNote.connections[(int)Direction.N] && group.Contains(grid.GetNote(i, j - 1)))
                                        newGroups.Add(regroup(i, j - 1, group));

                                    if (selectNote.connections[(int)Direction.NE] && group.Contains(grid.GetNote(i + 1, j - 1)))
                                        newGroups.Add(regroup(i + 1, j - 1, group));

                                    foreach (List<OmniNote> newGroup in newGroups)
                                    {
                                        if (newGroup.Count() == 1)
                                            noteGroups.Remove(newGroup.ElementAt(0));
                                        else
                                        {
                                            foreach (OmniNote currentNote in newGroup)
                                            {
                                                noteGroups.Remove(currentNote);
                                                noteGroups.Add(currentNote, newGroup);
                                            }
                                        }
                                    }
                                }
                            }
                            grid.RemoveNote(i, j);
                            selectNote = null;
                        }
                        //color swatch selection
                        else if (boundRedLt.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.HIGH;
                            selectNote.Tone = Note.C;
                        }
                        else if (boundRedMd.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.MID;
                            selectNote.Tone = Note.C;
                        }
                        else if (boundRedDk.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.LOW;
                            selectNote.Tone = Note.C;
                        }
                        else if (boundOrangeLt.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.HIGH;
                            selectNote.Tone = Note.E;
                        }
                        else if (boundOrangeMd.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.MID;
                            selectNote.Tone = Note.E;
                        }
                        else if (boundOrangeDk.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.LOW;
                            selectNote.Tone = Note.E;
                        }
                        else if (boundGreenLt.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.HIGH;
                            selectNote.Tone = Note.G;
                        }
                        else if (boundGreenMd.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.MID;
                            selectNote.Tone = Note.G;
                        }
                        else if (boundGreenDk.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.LOW;
                            selectNote.Tone = Note.G;
                        }
                        else if (boundTealLt.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.HIGH;
                            selectNote.Tone = Note.B;
                        }
                        else if (boundTealMd.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.MID;
                            selectNote.Tone = Note.B;
                        }
                        else if (boundTealDk.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            selectNote.Octave = Scale.LOW;
                            selectNote.Tone = Note.B;
                        }
                        else if (boundResize.Contains(new Vector3((float)x, (float)y, 0f)) == ContainmentType.Contains)
                        {
                            bResize = true;
                        }
                        else
                        {
                            bMenu = false;
                            selectCell = GetCellFromCoords(x, y);
                        }
                        
                    }
                    else
                    {
                        var cell = GetCellFromCoords(currentMouseState.X, currentMouseState.Y);
                        int i = cell.Item1;
                        int j = cell.Item2;

                        if (i < grid.NumRows && j < grid.NumRows && !grid.IsEmpty(i, j))
                            selectNote = grid.GetNote(i, j);

                        selectCell = new Tuple<int, int>(i, j);
                    }
                }
                //left pressed --> PRESSED  (hold)
                else if ((currentMouseState.LeftButton == ButtonState.Pressed) && (prevMouseState.LeftButton == ButtonState.Pressed))
                {
                    //moved?
                    if (prevMouseState.X != currentMouseState.X || prevMouseState.Y != currentMouseState.Y)
                    {
                        //menu
                        if (bMenu)
                        {
                            //resizing
                            if (bResize)
                            {
                                Rectangle cell = GetRectFromCell(selectCell.Item1, selectCell.Item2);
                                Vector2 origin = new Vector2(cell.Center.X, cell.Center.Y);
                                Vector2 cur = new Vector2(currentMouseState.X, currentMouseState.Y);
                                Vector2 prev = new Vector2(prevMouseState.X, prevMouseState.Y);

                                float prevLen = (prev - origin).Length();
                                float curLen = (cur - origin).Length();
                                float offset = curLen - prevLen;

                                float step = 0.01f;

                                if (offset < 0)
                                    selectNote.Amplitude += offset * step;
                                else if (offset > 0)
                                    selectNote.Amplitude += offset * step;

                                if (selectNote.Amplitude > 0.99f)
                                    selectNote.Amplitude = 0.99f;
                                else if (selectNote.Amplitude < 0.1f)
                                    selectNote.Amplitude = .1f;
                            }
                        }
                        else
                        {
                            if (selectCell != null)
                                bDragging = true;
                        }
                    }
                }
                //right released --> PRESSED 
                else if ((currentMouseState.RightButton == ButtonState.Pressed) && (prevMouseState.RightButton == ButtonState.Released))
                {
                    var cell = GetCellFromCoords(currentMouseState.X, currentMouseState.Y);
                    int i = cell.Item1;
                    int j = cell.Item2;

                    
                    selectCell = new Tuple<int, int>(i, j);

                    if (!grid.IsEmpty(i, j))
                    {
                        selectNote = grid.GetNote(i, j);

                        bMenu = true;
                        bConnecting = true;
                    }
                    else
                    {
                        bMenu = false;
                        bConnecting = false;
                    }
                }
                //right pressed --> PRESSED (hold)
                else if ((currentMouseState.RightButton == ButtonState.Pressed) && (prevMouseState.RightButton == ButtonState.Pressed))
                {
                    int offsetX = Math.Abs(currentMouseState.X - prevMouseState.X);
                    int offsetY = Math.Abs(currentMouseState.Y - prevMouseState.Y);

                    //moved?
                    if (offsetX > 5 || offsetY > 5)
                    {
                        bMenu = false;
#if LOOPINGVECTORS
                        if (!bConnecting && selectCell != null)
                            bDragging = true;
#endif
                    }
                }
                //right pressed --> RELEASED 
                else if ((currentMouseState.RightButton == ButtonState.Released) && (prevMouseState.RightButton == ButtonState.Pressed))
                {
                    if (bConnecting)
                    {
                        bConnecting = false;
                        int i = selectCell.Item1;
                        int j = selectCell.Item2;

                        var cell = GetCellFromCoords(currentMouseState.X, currentMouseState.Y);
                        int con_i = cell.Item1;
                        int con_j = cell.Item2;

                        int di = con_i - i;
                        int dj = con_j - j;

                        //check for note to connect to
                        if (!grid.IsEmpty(con_i, con_j))
                        {
                            //not conncting to itself
                            if ((di != 0) || (dj != 0))
                            {
                                //only one index at most displacement
                                if ((Math.Abs(di) <= 1) && (Math.Abs(dj) <= 1))
                                {
                                    ConnectNotes(selectNote, grid.GetNote(con_i, con_j));

                                    //EAST
                                    if (di > 0)
                                    {
                                        //NE
                                        if (dj < 0)
                                        {
                                            selectNote.connections[(int)Direction.NE] = true;
                                            grid.GetNote(con_i, con_j).connections[(int)Direction.SW] = true;
                                        }
                                        //SE
                                        else if (dj > 0)
                                        {
                                            selectNote.connections[(int)Direction.SE] = true;
                                            grid.GetNote(con_i, con_j).connections[(int)Direction.NW] = true;
                                        }
                                        //E
                                        else
                                        {
                                            selectNote.connections[(int)Direction.E] = true;
                                            grid.GetNote(con_i, con_j).connections[(int)Direction.W] = true;
                                        }
                                    }
                                    //WEST
                                    else if (di < 0)
                                    {
                                        //NW
                                        if (dj < 0)
                                        {
                                            selectNote.connections[(int)Direction.NW] = true;
                                            grid.GetNote(con_i, con_j).connections[(int)Direction.SE] = true;
                                        }
                                        //SW
                                        else if (dj > 0)
                                        {
                                            selectNote.connections[(int)Direction.SW] = true;
                                            grid.GetNote(con_i, con_j).connections[(int)Direction.NE] = true;
                                        }
                                        //W
                                        else
                                        {
                                            selectNote.connections[(int)Direction.W] = true;
                                            grid.GetNote(con_i, con_j).connections[(int)Direction.E] = true;
                                        }
                                    }
                                    else
                                    {
                                        //N
                                        if (dj < 0)
                                        {
                                            selectNote.connections[(int)Direction.N] = true;
                                            grid.GetNote(con_i, con_j).connections[(int)Direction.S] = true;
                                        }
                                        //S
                                        else
                                        {
                                            selectNote.connections[(int)Direction.S] = true;
                                            grid.GetNote(con_i, con_j).connections[(int)Direction.N] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
#if LOOPINGVECTORS
                    else if (bDragging && selectCell != null)
                    {
                        fireVector(gameTime, true);
                    }
#endif
                }


                prevKeyState = Keyboard.GetState();
                prevMouseState = currentMouseState;
            }

            base.Update(gameTime);
        }

        private List<OmniNote> regroup(int x, int y, List<OmniNote> group)
        {
            List<OmniNote> newGroup = new List<OmniNote>();
            List<Tuple<int, int>> queue = new List<Tuple<int, int>>();
            queue.Add(new Tuple<int, int>(x, y));
            group.Remove(grid.GetNote(x, y));
            while (queue.Count() > 0)
            {
                Tuple<int, int> currentLocation = queue.ElementAt(0);
                int i = currentLocation.Item1;
                int j = currentLocation.Item2;
                queue.RemoveAt(0);
                OmniNote currentNote = grid.GetNote(i, j);
                newGroup.Add(currentNote);
                if (currentNote.connections[(int)Direction.E] && group.Contains(grid.GetNote(i + 1, j)))
                {
                    group.Remove(grid.GetNote(i + 1, j));
                    queue.Add(new Tuple<int, int>(i + 1, j));
                }
                if (currentNote.connections[(int)Direction.SE] && group.Contains(grid.GetNote(i + 1, j + 1)))
                {
                    group.Remove(grid.GetNote(i + 1, j + 1));
                    queue.Add(new Tuple<int, int>(i + 1, j + 1));
                }
                if (currentNote.connections[(int)Direction.S] && group.Contains(grid.GetNote(i, j + 1)))
                {
                    group.Remove(grid.GetNote(i, j + 1));
                    queue.Add(new Tuple<int, int>(i, j + 1));
                }
                if (currentNote.connections[(int)Direction.SW] && group.Contains(grid.GetNote(i - 1, j + 1)))
                {
                    group.Remove(grid.GetNote(i - 1, j + 1));
                    queue.Add(new Tuple<int, int>(i - 1, j + 1));
                }
                if (currentNote.connections[(int)Direction.W] && group.Contains(grid.GetNote(i - 1, j)))
                {
                    group.Remove(grid.GetNote(i - 1, j));
                    queue.Add(new Tuple<int, int>(i - 1, j));
                }
                if (currentNote.connections[(int)Direction.NW] && group.Contains(grid.GetNote(i - 1, j - 1)))
                {
                    group.Remove(grid.GetNote(i - 1, j - 1));
                    queue.Add(new Tuple<int, int>(i - 1, j - 1 ));
                }
                if (currentNote.connections[(int)Direction.N] && group.Contains(grid.GetNote(i, j - 1)))
                {
                    group.Remove(grid.GetNote(i, j - 1));
                    queue.Add(new Tuple<int, int>(i, j - 1));
                }
                if (currentNote.connections[(int)Direction.NE] && group.Contains(grid.GetNote(i + 1, j - 1)))
                {
                    group.Remove(grid.GetNote(i + 1, j - 1));
                    queue.Add(new Tuple<int, int>(i + 1, j - 1));
                }
            }
            return newGroup;
        }

        private int GetCellWidth()
        {
            return graphics.GraphicsDevice.Viewport.Width / grid.NumRows;
        }

        private int GetCellHeight()
        {
            return graphics.GraphicsDevice.Viewport.Height / grid.NumRows;
        }


        private Tuple<int, int> GetCellFromCoords(int x, int y)
        {
            int stepX = GetCellWidth();
            int stepY = GetCellHeight();

            return Tuple.Create<int, int>(x / stepX, y / stepY);
        }

        private Point GetCoordsFromCell(int i, int j)
        {
            int stepX = GetCellWidth();
            int stepY = GetCellHeight();

            return new Point(stepX * i, stepY * j);
        }

        private Rectangle GetRectFromCell(int i, int j)
        {
            Point coords = GetCoordsFromCell(i, j);
            return new Rectangle(coords.X, coords.Y, GetCellWidth(), GetCellHeight());
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(new Color(230,230,240));

            DrawActiveCells();
            DrawGrid();
            DrawNotes();

            //draw trash can
            Rectangle rect = new Rectangle();
            rect.Width = (int)((float)texTrash.Width * .6);
            rect.Height = (int)((float)texTrash.Height * .6);
            rect.X = 2;
            rect.Y = graphics.PreferredBackBufferWidth - rect.Height;
            boundTrash = rect;
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(texTrash, boundTrash, Color.White);
            spriteBatch.End();

            if (bConnecting)
                DrawConnecting();

            if (bDragging)
                DrawBand();

            base.Draw(gameTime);
        }

        private void DrawActiveCells()
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            foreach (Vector v in vectors)
            {
                Rectangle cell = GetRectFromCell(v.xPosition, v.yPosition);
                spriteBatch.Draw(texPix, cell, null, new Color(0, 0, 0, 35), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            }

            spriteBatch.End();
        }

        private void DrawConnecting()
        {
            Point anchor = GetRectFromCell(selectCell.Item1, selectCell.Item2).Center;
            int x = currentMouseState.X;
            int y = currentMouseState.Y;

            Color c = Color.White;
            primitiveBatch.Begin(PrimitiveType.LineList);
            primitiveBatch.AddVertex(new Vector2(anchor.X, anchor.Y), c);
            primitiveBatch.AddVertex(new Vector2(x, y), c);
            primitiveBatch.End();
        }

        private void DrawBand()
        {
            Point anchor = GetRectFromCell(selectCell.Item1, selectCell.Item2).Center;
            int x = currentMouseState.X;
            int y = currentMouseState.Y;

            int offX = x - anchor.X;
            int offY = y - anchor.Y;

            //Now offX and offY are known to both be nonzero
            double tangent = (double)offY / (double)offX;

            int length = (int)Math.Sqrt(offX * offX + offY * offY);
            int maxLength = GetCellWidth() * 3;
            //use max if over limit
            length = Math.Min(length, maxLength);
            int side = (int)Math.Sqrt(length * length / 2);

            if (offX != 0 || offY != 0)
            {
                //Handle perfectly vertical line
                if (offX == 0)
                {
                    if (offY > 0)
                    {
                        currDirection = Direction.N;
                        y = anchor.Y + length;
                    }
                    else
                    {
                        currDirection = Direction.S;
                        y = anchor.Y - length;
                    }
                }

                //Handle perfectly horizontal line
                else if (offY == 0)
                {
                    if (offX > 0)
                    {
                        currDirection = Direction.W;
                        x = anchor.X + length;
                    }
                    else
                    {
                        currDirection = Direction.E;
                        x = anchor.X - length;
                    }
                }


                //Handle fourth quadrant (postive offX, positive offY)
                else if (offX > 0 && offY > 0)
                {
                    //First 22.5 degrees
                    if (tangent < TAN_22_5)
                    {
                        y = anchor.Y;
                        x = anchor.X + length;
                        currDirection = Direction.W;
                    }

                    //Middle 45 degrees
                    else if (tangent < TAN_67_5)
                    {
                        x = anchor.X + side;
                        y = anchor.Y + side;
                        currDirection = Direction.NW;
                    }

                    //Last 22.5 degrees
                    else
                    {
                        x = anchor.X;
                        y = anchor.Y + length;
                        currDirection = Direction.N;
                    }
                }

                //Handle third quadrant (negative offX, positive offY)
                else if (offX < 0 && offY > 0)
                {
                    tangent *= -1;
                    //First 22.5 degrees
                    if (tangent > TAN_67_5)
                    {
                        x = anchor.X;
                        y = anchor.Y + length;
                        currDirection = Direction.N;
                    }

                    //Middle 45 degrees
                    else if (tangent > TAN_22_5)
                    {
                        x = anchor.X - side;
                        y = anchor.Y + side;
                        currDirection = Direction.NE;
                    }

                    //Last 22.5 degrees
                    else
                    {
                        y = anchor.Y;
                        x = anchor.X - length;
                        currDirection = Direction.E;    
                    }
                }

                //Handle second quadrant (negative offX, negative offY)
                else if (offX < 0 && offY < 0)
                {
                    //First 22.5 degrees
                    if (tangent < TAN_22_5)
                    {
                        y = anchor.Y;
                        x = anchor.X - length;
                        currDirection = Direction.E;
                    }

                    //Middle 45 degrees
                    else if (tangent < TAN_67_5)
                    {
                        x = anchor.X - side;
                        y = anchor.Y - side;
                        currDirection = Direction.SE;
                    }

                    //Last 22.5 degrees
                    else
                    {
                        x = anchor.X;
                        y = anchor.Y - length;
                        currDirection = Direction.S;
                    }
                }

                //Handle first quadrant (positive offX, negative offY)
                else if (offX > 0 && offY < 0)
                {
                    tangent *= -1;
                    //First 22.5 degrees
                    if (tangent > TAN_67_5)
                    {
                        x = anchor.X;
                        y = anchor.Y - length;
                        currDirection = Direction.S;
                    }

                    //Middle 45 degrees
                    else if (tangent > TAN_22_5)
                    {
                        x = anchor.X + side;
                        y = anchor.Y - side;
                        currDirection = Direction.SW;
                    }

                    //Last 22.5 degrees
                    else
                    {
                        y = anchor.Y;
                        x = anchor.X + length;
                        currDirection = Direction.W;
                    }
                }
            }


            dragRatio = (double)length / (double)maxLength;
            Color bandColor = new Color((int)(dragRatio * 255), (int)((1 - dragRatio) * 255), 0);
            primitiveBatch.Begin(PrimitiveType.LineList);
            primitiveBatch.AddVertex(new Vector2(anchor.X - 5, anchor.Y), bandColor);
            primitiveBatch.AddVertex(new Vector2(x, y), bandColor);
            primitiveBatch.End();
            primitiveBatch.Begin(PrimitiveType.LineList);
            primitiveBatch.AddVertex(new Vector2(anchor.X + 5, anchor.Y), bandColor);
            primitiveBatch.AddVertex(new Vector2(x, y), bandColor);
            primitiveBatch.End();
        }

        private void DrawNotes()
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            for (int i = 0; i < grid.NumRows; i++)
                for (int j = 0; j < grid.NumRows; j++)
                {
                    if (!grid.IsEmpty(i, j))
                    {
                        OmniNote note = grid.GetNote(i, j);
                        double amp = note.Amplitude;

                        Rectangle picRect = GetRectFromCell(i, j);
                        //correct aspect ratio (100% size)
                        picRect.Width = (int)((double)picRect.Width * ((double)GetCellHeight() / (double)GetCellWidth()));

                        //adjust size
                        picRect.Width = (int)((double)picRect.Width * amp);
                        picRect.Height = (int)((double)picRect.Height * amp);

                        //correct position
                        int offX = (GetCellWidth() - picRect.Width) / 2;
                        int offY = (GetCellHeight() - picRect.Height) / 2;
                        picRect.X += offX;
                        picRect.Y += offY;


                        Texture2D tex = null;
                        bool active = note.activeVectors > 0;

                        //Ugly, Large switch statement. Hash map would be better...
                        switch (note.Tone)
                        {
                            case Note.C:
                                switch (note.Octave)
                                {
                                    case Scale.LOW:
                                        if (active)
                                            tex = c_low_on;
                                        else
                                            tex = c_low_off;
                                        break;
                                    case Scale.MID:
                                        if (active)
                                            tex = c_mid_on;
                                        else
                                            tex = c_mid_off;
                                        break;
                                    case Scale.HIGH:
                                        if (active)
                                            tex = c_high_on;
                                        else
                                            tex = c_high_off;
                                        break;
                                }
                                break;

                            case Note.E:
                                switch (note.Octave)
                                {
                                    case Scale.LOW:
                                        if (active)
                                            tex = e_low_on;
                                        else
                                            tex = e_low_off;
                                        break;
                                    case Scale.MID:
                                        if (active)
                                            tex = e_mid_on;
                                        else
                                            tex = e_mid_off;
                                        break;
                                    case Scale.HIGH:
                                        if (active)
                                            tex = e_high_on;
                                        else
                                            tex = e_high_off;
                                        break;
                                }
                                break;

                            case Note.G:
                                switch (note.Octave)
                                {
                                    case Scale.LOW:
                                        if (active)
                                            tex = g_low_on;
                                        else
                                            tex = g_low_off;
                                        break;
                                    case Scale.MID:
                                        if (active)
                                            tex = g_mid_on;
                                        else
                                            tex = g_mid_off;
                                        break;
                                    case Scale.HIGH:
                                        if (active)
                                            tex = g_high_on;
                                        else
                                            tex = g_high_off;
                                        break;
                                }
                                break;

                            case Note.B:
                                switch (note.Octave)
                                {
                                    case Scale.LOW:
                                        if (active)
                                            tex = b_low_on;
                                        else
                                            tex = b_low_off;
                                        break;
                                    case Scale.MID:
                                        if (active)
                                            tex = b_mid_on;
                                        else
                                            tex = b_mid_off;
                                        break;
                                    case Scale.HIGH:
                                        if (active)
                                            tex = b_high_on;
                                        else
                                            tex = b_high_off;
                                        break;
                                }
                                break;
                        }

                        for (int k = 0; k < note.connections.Length; k++)
                        {
                            bool connect = note.connections[k];
                            int ix = i;
                            int jx = j;

                            switch (k)
                            {
                                case (int)Direction.E:
                                    if (connect)
                                        ix++;
                                    break;
                                case (int)Direction.N:
                                    if (connect)
                                        jx--;
                                    break;
                                case (int)Direction.NE:
                                    if (connect)
                                    {
                                        ix++;
                                        jx--;
                                    }
                                    break;
                                case (int)Direction.NW:
                                    if (connect)
                                    {
                                        ix--;
                                        jx--;
                                    }
                                    break;
                                case (int)Direction.S:
                                    if (connect)
                                        jx++;
                                    break;
                                case (int)Direction.SE:
                                    if (connect)
                                    {
                                        jx++;
                                        ix++;
                                    }
                                    break;
                                case (int)Direction.SW:
                                    if (connect)
                                    {
                                        jx++;
                                        ix--;
                                    }
                                    break;
                                case (int)Direction.W:
                                    if (connect)
                                        ix--;
                                    break;
                            }

                            Color c = Color.Purple;
                            Rectangle cell1 = GetRectFromCell(i, j);
                            Rectangle cell2 = GetRectFromCell(ix, jx);
                            primitiveBatch.Begin(PrimitiveType.LineList);
                            primitiveBatch.AddVertex(new Vector2(cell1.Center.X, cell1.Center.Y), c);
                            primitiveBatch.AddVertex(new Vector2(cell2.Center.X, cell2.Center.Y), c);
                            primitiveBatch.End();
                        }

                        
                        //IF SELECTED note
                        if (note == selectNote)
                        {
                            //Draw line through cell
                            Color gridColor = new Color(150, 150, 150);
                            primitiveBatch.Begin(PrimitiveType.LineList);
                            primitiveBatch.AddVertex(new Vector2(picRect.X, picRect.Y), gridColor);
                            primitiveBatch.AddVertex(new Vector2(picRect.X + picRect.Width, picRect.Y + picRect.Height), gridColor);
                            primitiveBatch.End();

                            if (bMenu)
                                DrawMenu(i, j);

                            spriteBatch.Draw(tex, picRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, .1f);

                        }
                        else
                            spriteBatch.Draw(tex, picRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    }
                }
            
            spriteBatch.End();
        }

        private void DrawMenu(int i, int j)
        {
            Rectangle rect = GetRectFromCell(i, j);
            Rectangle rectEdge = new Rectangle(rect.Center.X, rect.Center.Y,(int)( rect.Width * 0.75), 20);
            var menuRots = GetMenuRotations(i, j);
            //rotations
            float delRot = menuRots.Item1;
            float colRot = menuRots.Item2;
            float szRot = menuRots.Item3;

            //Draw edges
            spriteBatch.Draw(texMenuBar, rectEdge, null, Color.White, delRot, new Vector2(0, 15), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texMenuBar, rectEdge, null, Color.White, colRot, new Vector2(0, 15), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texMenuBar, rectEdge, null, Color.White, szRot, new Vector2(0, 15), SpriteEffects.None, 0.2f);

            int length = (int)(rectEdge.Width * 1.22);
            
            //Delete Icon
            float ratio = 0.35f;
            int width = (int)(texDelete.Width * ratio);
            int height = (int)(texDelete.Height * ratio);
            int x = rect.Center.X - width/2;
            int y = rect.Center.Y - height/2;

            boundDel = new Rectangle((int)(length * Math.Cos(delRot)) + x, (int)(length * Math.Sin(delRot)) + y, width, height);
            spriteBatch.Draw(texDelete, boundDel, new Color(255, 255, 255, 220));

            //Color Menu
            ratio = 0.20f;
            width = (int)(GetCellWidth() * ratio);
            height = (int)(GetCellHeight() * ratio);
            x = rect.Center.X;
            y = rect.Center.Y;
            Rectangle square = new Rectangle(x ,y, width, height);
            length = (int)(rectEdge.Width * 1.20);

            //more efficient bounding can probably be achieved through matrix transformation
            //lots of cos and sin calls..
            boundRedLt = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot - .23)) + x, (float)(length * Math.Sin(colRot - .23)) + y, 0f), width / 2);
            boundRedMd = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot)) + x, (float)(length * Math.Sin(colRot)) + y, 0f), width / 2);
            boundRedDk = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot + .23)) + x, (float)(length * Math.Sin(colRot + .23)) + y, 0f), width / 2);
            length += (int)(width * 1.23f);
            boundOrangeLt = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot - .2)) + x, (float)(length * Math.Sin(colRot - .2)) + y, 0f), width / 2);
            boundOrangeMd = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot)) + x, (float)(length * Math.Sin(colRot)) + y, 0f), width / 2);
            boundOrangeDk = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot + .2)) + x, (float)(length * Math.Sin(colRot + .2)) + y, 0f), width / 2);
            length += (int)(width * 1.23f);
            boundGreenLt = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot - .17)) + x, (float)(length * Math.Sin(colRot - .17)) + y, 0f), width / 2);
            boundGreenMd = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot)) + x, (float)(length * Math.Sin(colRot)) + y, 0f), width / 2);
            boundGreenDk = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot + .17)) + x, (float)(length * Math.Sin(colRot + .17)) + y, 0f), width / 2);
            length += (int)(width * 1.23f);
            boundTealLt = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot - .14)) + x, (float)(length * Math.Sin(colRot - .14)) + y, 0f), width / 2);
            boundTealMd = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot)) + x, (float)(length * Math.Sin(colRot)) + y, 0f), width / 2);
            boundTealDk = new BoundingSphere(new Vector3((float)(length * Math.Cos(colRot + .14)) + x, (float)(length * Math.Sin(colRot + .14)) + y, 0f), width / 2);

            //draw swatches
            spriteBatch.Draw(texPix, square, null, new Color(235, 75, 64, 220), colRot, new Vector2(-3.9f, .5f), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texPix, square, null, new Color(255, 84, 72, 200), colRot, new Vector2(-3.9f, 1.6f), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texPix, square, null, new Color(207, 66, 56, 240), colRot, new Vector2(-3.9f, -.6f), SpriteEffects.None, 0.2f);

            spriteBatch.Draw(texPix, square, null, new Color(231, 140, 29, 220), colRot, new Vector2(-5.1f, .5f), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texPix, square, null, new Color(255, 156, 33, 200), colRot, new Vector2(-5.1f, 1.6f), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texPix, square, null, new Color(207, 126, 26, 240), colRot, new Vector2(-5.1f, -.6f), SpriteEffects.None, 0.2f);

            spriteBatch.Draw(texPix, square, null, new Color(175, 218, 32, 220), colRot, new Vector2(-6.3f, .5f), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texPix, square, null, new Color(194, 242, 36, 200), colRot, new Vector2(-6.3f, 1.6f), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texPix, square, null, new Color(155, 194, 28, 240), colRot, new Vector2(-6.3f, -.6f), SpriteEffects.None, 0.2f);

            spriteBatch.Draw(texPix, square, null, new Color(40, 216, 169, 220), colRot, new Vector2(-7.5f, .5f), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texPix, square, null, new Color(44, 240, 188, 200), colRot, new Vector2(-7.5f, 1.6f), SpriteEffects.None, 0.2f);
            spriteBatch.Draw(texPix, square, null, new Color(36, 192, 150, 240), colRot, new Vector2(-7.5f, -.6f), SpriteEffects.None, 0.2f);

            //resize
            ratio = 0.17f;
            width = (int)(GetCellWidth() * ratio);
            height = (int)(GetCellHeight() * ratio);
            x = rect.Center.X - width /2;
            y = rect.Center.Y - height /2;

            length = (int)((rectEdge.Width * 0.9) + (GetCellWidth() * selectNote.Amplitude));
            Rectangle circle = new Rectangle((int)(length * Math.Cos(szRot)) + x, (int)(length * Math.Sin(szRot)) + y, width, height);
            boundResize = new BoundingSphere(new Vector3(circle.X, circle.Y, 0f), circle.Width);

            spriteBatch.Draw(texCircle, circle, new Color(0, 0, 0, 150));
        }

        private Tuple<float, float, float> GetMenuRotations(int i, int j)
        {
            float PI = (float)Math.PI;

            //LEFT side
            if (i < 1)
            {
                //TOP-LEFT
                if (j < 1)
                    return new Tuple<float, float, float>(PI / 2, 0.0f, PI / 4);

                //BOTTOM-LEFT
                else if (j >= grid.NumRows - 1)
                    return new Tuple<float, float, float>(3*PI / 2, 0.0f, 7*PI / 4);

                //LEFT
                else
                    return new Tuple<float, float, float>(PI / 4, 0.0f, 7* PI / 4);
            }
            //RIGHT side
            else if (i >= grid.NumRows - 1)
            {
                //TOP-RIGHT
                if (j < 1)
                    return new Tuple<float, float, float>(PI / 2, PI, 3*PI / 4);

                //BOTTOM-RIGHT
                else if (j >= grid.NumRows - 1)
                    return new Tuple<float, float, float>(3*PI / 2, PI, 5 * PI / 4);

                //RIGHT
                else
                    return new Tuple<float, float, float>(3* PI / 4, PI, 5 * PI / 4);
            }
            //MIDDLE 
            else
            {
                //TOP
                if (j < 1)
                    return new Tuple<float, float, float>(3 *PI / 4, PI / 2, PI / 4);

                //BOTTOM
                else if (j >= grid.NumRows - 1)
                    return new Tuple<float, float, float>(5*PI / 4, 3*PI/2, 7 * PI / 4);

                //CENTER
                else
                    return new Tuple<float, float, float>(PI / 2, 5*PI /4, 7 * PI / 4);
            }
        }

        private void DrawGrid()
        {
            int rows = grid.NumRows;
            int width = graphics.GraphicsDevice.Viewport.Width;
            int height = graphics.GraphicsDevice.Viewport.Height;

            int stepX = width / rows;
            int stepY = height / rows;

            primitiveBatch.Begin(PrimitiveType.LineList);
            Color gridColor = new Color(150, 150, 150);

            for (int i = 0; i <= rows; i++)
            {
                //horizontal lines
                primitiveBatch.AddVertex(new Vector2(0, i*stepY), gridColor);
                primitiveBatch.AddVertex(new Vector2(width, i*stepY), gridColor);

                //vertical lines
                primitiveBatch.AddVertex(new Vector2(i*stepX, 0), gridColor);
                primitiveBatch.AddVertex(new Vector2(i*stepX, height), gridColor);
            }

            primitiveBatch.End();
        }
        
        private static List<SoundEffectInstance> PlayNote(OmniNote note, int time)
        {
            List<SoundEffectInstance> samples = new List<SoundEffectInstance>();

            if (note == null)
                return samples;

            List<OmniNote> notesToPlay;
            if (!noteGroups.TryGetValue(note, out notesToPlay))
            {
                notesToPlay = new List<OmniNote>();
                notesToPlay.Add(note);
            }
            SoundEffectInstance sample;
            int pitch;
            foreach (OmniNote currentNote in notesToPlay)
            {
                switch (currentNote.Octave)
                {
                    case Scale.LOW: pitch = 0; break;
                    case Scale.MID: pitch = 1; break;
                    case Scale.HIGH: pitch = 2; break;
                    default:
                        pitch = 1; break;
                }

                switch (currentNote.Tone)
                {
                    case Note.B: sample = bNote[pitch][time / 200 - 1].CreateInstance(); break;
                    case Note.C: sample = cNote[pitch][time / 200 - 1].CreateInstance(); break;
                    case Note.E: sample = eNote[pitch][time / 200 - 1].CreateInstance(); break;
                    case Note.G: sample = gNote[pitch][time / 200 - 1].CreateInstance(); break;
                    default:
                        sample = cNote[pitch][time / 200 - 1].CreateInstance(); break;
                }

                sample.Pitch = 0;
                sample.Volume = (float)currentNote.Amplitude;
                sample.Play();

                samples.Add(sample);
            }

            return samples;
        }

        private void ConnectNotes(OmniNote note1, OmniNote note2)
        {
            //If the notes are already connected, do nothing
            if (noteGroups.ContainsKey(note1))
            {
                List<OmniNote> grp;
                noteGroups.TryGetValue(note1, out grp);

                if (grp.Contains(note2))
                    return;
            }

            //Get the existing groups for each note
            List<OmniNote> group1, group2;
            noteGroups.TryGetValue(note1, out group1);
            noteGroups.TryGetValue(note2, out group2);

            //Case where neither note is part of a group
            if (group1 == null && group2 == null)
            {
                group1 = new List<OmniNote>();
                group1.Add(note1);
                group1.Add(note2);
                noteGroups.Add(note1, group1);
                noteGroups.Add(note2, group1);
            }
            
            //Case where only note1 is part of a group
            else if (group1 != null && group2 == null)
            {
                //noteGroups.Remove(note1);
                group1.Add(note2);
                //noteGroups.Add(note1, group1);
                noteGroups.Add(note2, group1);
            }

            //Case where only note2 is part of a group
            else if (group1 == null && group2 != null)
            {
                //noteGroups.Remove(note2);
                group2.Add(note1);
                noteGroups.Add(note1, group2);
                //noteGroups.Add(note2, group2);
            }

            //Case where both notes are part of existing groups
            else
            {
                //Check if the notes are already part of the same group
                if (group1.Contains(note2))
                    return;

                //If not, then merge the contents of group2 into group1, and set all the group2 notes to point to group1 in the dictionary
                foreach (OmniNote note in group2)
                {
                    group1.Add(note);
                    noteGroups.Remove(note);
                    noteGroups.Add(note, group1);
                }
            }
        }

        //Send out a vector
        private void fireVector(GameTime gameTime, bool looping)
        {
            bDragging = false;

            //Shoot Vector
            Direction vectorDirection = currDirection;
            int vectorStartTime = (int)(gameTime.TotalGameTime.TotalMilliseconds);
            //Console.WriteLine("Time: {0}, Cell X: {1}, Cell y: {2}, Direction: {3}", vectorStartTime, selectCell.Item1, selectCell.Item2, reverseDirection(currDirection));
            int xDir, yDir;
            switch (vectorDirection)
            {
                case Direction.E: xDir = 1; yDir = 0; break;
                case Direction.SE: xDir = 1; yDir = 1; break;
                case Direction.S: xDir = 0; yDir = 1; break;
                case Direction.SW: xDir = -1; yDir = 1; break;
                case Direction.W: xDir = -1; yDir = 0; break;
                case Direction.NW: xDir = -1; yDir = -1; break;
                case Direction.N: xDir = 0; yDir = -1; break;
                case Direction.NE: xDir = 1; yDir = -1; break;
                default: xDir = 1; yDir = 0; break;
            }

            Vector newVector = new Vector(selectCell.Item1, selectCell.Item2, xDir, yDir, (int)((1 - dragRatio) * 5) * 200 + 200, vectorStartTime, vectorDirection, looping);
            vectors.Add(newVector);
        }

        private class Vector
        {
            //The vector's current position on the x-axis
            public int xPosition{ get; set; }

            //The vector's current position on the y-axis
            public int yPosition{ get; set; }

            //The vector's direction of movement along the x-axis; either -1, 0, or 1
            private int xDirection;

            //The vector's direction of movement along the y-axis; either -1, 0, or 1
            private int yDirection;

            //The vector's direction of movement
            private Direction direction;

            //Number of milliseconds the vector takes to travel one graph increment
            private int millisecondsPerMove;

            //Game time in milliseconds when the vector was launched
            private int startTime;

            //Number of times the vector has moved
            private int movements;

            private bool bLooping;

            public List<SoundEffectInstance> effectsToDispose { get; set; }

            public Vector(int newX, int newY, int newXDir, int newYDir, int newMPM, int newStartTime, Direction dir, bool looping)
            {
                xPosition = newX;
                yPosition = newY;
                xDirection = newXDir;
                yDirection = newYDir;
                direction = dir;
                millisecondsPerMove = newMPM;
                startTime = newStartTime;
                movements = 0;
                bLooping = looping;
                OmniNote note = grid.GetNote(xPosition, yPosition);
                effectsToDispose = new List<SoundEffectInstance>();
                if (note != null)
                {
                    List<OmniNote> notesToActivate;
                    if (!noteGroups.TryGetValue(note, out notesToActivate))
                    {
                        notesToActivate = new List<OmniNote>();
                        notesToActivate.Add(note);
                    }
                    foreach (OmniNote currentNote in notesToActivate)
                        currentNote.activeVectors++;
                    effectsToDispose.AddRange(PlayNote(note, millisecondsPerMove));
                }
            }

            public bool UpdatePosition(int currentTime)
            {
                int totalTime = currentTime - startTime;
                if ((int)(totalTime / millisecondsPerMove) > movements)
                {
                    OmniNote note = grid.GetNote(xPosition, yPosition);
                    if (note != null)
                    {
                        List<OmniNote> notesToDeactivate;
                        if (!noteGroups.TryGetValue(note, out notesToDeactivate))
                        {
                            notesToDeactivate = new List<OmniNote>();
                            notesToDeactivate.Add(note);
                        }
                        foreach (OmniNote currentNote in notesToDeactivate)
                            currentNote.activeVectors--;
                    }
                    
                    //Save the current location before setting the future location
                    int currx = xPosition;
                    int curry = yPosition;  //yum

                    xPosition += xDirection;
                    yPosition += yDirection;
                    movements++;

                    //Check for vector reaching the edge of the grid
                    if (xPosition < 0 || xPosition >= grid.NumRows || yPosition < 0 || yPosition >= grid.NumRows)
                    {
#if LOOPINGVECTORS
                        if (!bLooping)
                            return false;

                        //Otherwise looping - Move it to the opposite side of the grid
                        int gridMax = grid.NumRows - 1;
                        switch (direction)
                        {
                            case Direction.E:
                                xPosition = 0;
                                break;
                            case Direction.SE:
                                xPosition = gridMax - curry;
                                yPosition = gridMax - currx;
                                break;
                            case Direction.S:
                                yPosition = 0;
                                break;
                            case Direction.SW:
                                xPosition = curry;
                                yPosition = currx;
                                break;
                            case Direction.W:
                                xPosition = gridMax;
                                break;
                            case Direction.NW:
                                xPosition = gridMax - curry;
                                yPosition = gridMax - currx;
                                break;
                            case Direction.N:
                                yPosition = gridMax;
                                break;
                            case Direction.NE:
                                xPosition = curry;
                                yPosition = currx;
                                break;
                            default:
                                break;
                        }
                        return true;
#else
                        return false;
#endif
                    }

                    note = grid.GetNote(xPosition, yPosition);
                    if (note != null)
                    {
                        List<OmniNote> notesToActivate;
                        if (!noteGroups.TryGetValue(note, out notesToActivate))
                        {
                            notesToActivate = new List<OmniNote>();
                            notesToActivate.Add(note);
                        }
                        foreach (OmniNote currentNote in notesToActivate)
                            currentNote.activeVectors++;
                        effectsToDispose.AddRange(PlayNote(note, millisecondsPerMove));
                    }
                }
                return true;
            }
        }
    }
}
