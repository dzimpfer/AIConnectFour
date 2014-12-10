using ConnectFour.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ConnectFour
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public int BOARD_WIDTH = 6;
        public int BOARD_HEIGHT = 7;
        public bool firstPlayerTurn = true;
        public bool gameOver = false;
        public string firstPlayerName = "Player One";
        public string secondPlayerName = "Player Two";
        public Color firstPlayerColor = Colors.Blue;
        public Color secondPlayerColor = Colors.Red;
        public int firstPlayerScore = 0;
        public int secondPlayerScore = 0;
        public string[] topPlayers = { "One", "Two", "Three", "Four", "Five" };
        public int[] topPlayerScores = { 0, 0, 0, 0, 0 };
        public int[,] grid = { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
        //public int[,] grid = { { 0, 1, 2, 0, 1, 2 }, { 1, 0, 2, 2, 1, 0 }, { 2, 2, 1, 0, 0, 1 }, { 0, 1, 0, 2, 1, 2 }, { 0, 2, 2, 1, 0, 0 }, { 0, 2, 1, 2, 1, 0 }, { 1, 1, 2, 2, 0, 1 } };
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private DispatcherTimer timer = new DispatcherTimer();
        public int TWO_VALUE = 2;
        public int THREE_VALUE = 100;
        public int FOUR_VALUE = 10000;
        public int DEPTH = 5;
        public struct Play
        {
            public int column;
            public int score;
            public Play(int col, int sco)
            {
                column = col;
                score = sco;
            }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        #region GUI
        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    Border border = new Border();
                    border.Width = 100;
                    border.Height = border.Width;
                    Canvas.SetTop(border, 100 * r);
                    Canvas.SetLeft(border, 100 * c);
                    //border.Name = generateName(c, r);
                    //border.PointerPressed += showborder;
                    border.BorderBrush = new SolidColorBrush(Colors.White);
                    border.BorderThickness = new Thickness(1, 1, 1, 1);
                    if (r == 0 && c == 0)
                        border.CornerRadius = new CornerRadius(20, 0, 0, 0);
                    if (r == 0 && c == BOARD_WIDTH - 1)
                        border.CornerRadius = new CornerRadius(0, 20, 0, 0);
                    if (r == BOARD_HEIGHT - 1 && c == 0)
                        border.CornerRadius = new CornerRadius(0, 0, 0, 20);
                    if (r == BOARD_HEIGHT - 1 && c == BOARD_WIDTH - 1)
                        border.CornerRadius = new CornerRadius(0, 0, 20, 0);

                    if (c == 0)
                        border.BorderThickness = new Thickness(2, 1, 1, 1);
                    if (c == BOARD_WIDTH - 1)
                        border.BorderThickness = new Thickness(1, 1, 2, 1);
                    if (r == BOARD_HEIGHT - 1)
                        border.BorderThickness = new Thickness(1, 1, 1, 2);
                    if (r == 0)
                        border.BorderThickness = new Thickness(1, 2, 1, 1);

                    boardCanvas.Children.Add(border);
                }
            }

            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 80;
                    ellipse.Height = ellipse.Width;
                    Canvas.SetTop(ellipse, 100 * r + 10);
                    Canvas.SetLeft(ellipse, 100 * c + 10);
                    //rect.Name = generateName(c, r);
                    //rect.PointerPressed += showRect;
                    boardCanvas.Children.Add(ellipse);
                }
            }

            for (int c = 0; c < BOARD_WIDTH; c++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Height = 100;
                textBlock.Width = 100;
                Canvas.SetLeft(textBlock, 100 * c);
                textBlock.Text = (c + 1).ToString();
                textBlock.FontFamily = new FontFamily("Kristen ITC");
                textBlock.FontSize = 36;
                textBlock.TextAlignment = TextAlignment.Center;

                textBlock.Name = c.ToString();
                textBlock.PointerPressed += columnClicked;

                columnCanvas.Children.Add(textBlock);
            }

            columnCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            columnCanvas.Margin = new Thickness(0, 125, 0, 0);
            columnCanvas.Height = 100;
            columnCanvas.Width = BOARD_WIDTH * 100;

            boardCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            boardCanvas.VerticalAlignment = VerticalAlignment.Center;
            boardCanvas.Height = BOARD_HEIGHT * 100;
            boardCanvas.Width = BOARD_WIDTH * 100;
            DrawGrid();

            firstPlayerNameTextBlock.Text = firstPlayerName + ":  ";
            firstPlayerScoreTextBlock.Text = firstPlayerScore.ToString();

            secondPlayerNameTextBlock.Text = secondPlayerName + ":  ";
            secondPlayerScoreTextBlock.Text = secondPlayerScore.ToString();

            populateTopPlayers();

            if (firstPlayerTurn)
            {
                interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
                interactionTextBlock.Text = firstPlayerName + "'s Turn!";
            }
            else
            {
                interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
                interactionTextBlock.Text = secondPlayerName + "'s Turn!";
            }
            if (playerWon(grid, 1))
            {
                interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
                interactionTextBlock.Text = firstPlayerName + " Won!";
            }
            if (playerWon(grid, 2))
            {
                interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
                interactionTextBlock.Text = secondPlayerName + " Won!";
            }
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // Load session and app state
            Windows.Storage.ApplicationDataContainer roamingSettings =
        Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("firstPlayerName"))
                firstPlayerName = roamingSettings.Values["firstPlayerName"].ToString();

            if (roamingSettings.Values.ContainsKey("secondPlayerName"))
                secondPlayerName = roamingSettings.Values["secondPlayerName"].ToString();

            if (roamingSettings.Values.ContainsKey("firstPlayerScore"))
                firstPlayerScore = Convert.ToInt32(roamingSettings.Values["firstPlayerScore"].ToString());

            if (roamingSettings.Values.ContainsKey("secondPlayerScore"))
                secondPlayerScore = Convert.ToInt32(roamingSettings.Values["secondPlayerScore"].ToString());

            firstPlayerNameTextBlock.Text = firstPlayerName + ":  ";
            firstPlayerScoreTextBlock.Text = firstPlayerScore.ToString();

            secondPlayerNameTextBlock.Text = secondPlayerName + ":  ";
            secondPlayerScoreTextBlock.Text = secondPlayerScore.ToString();

            if (roamingSettings.Values.ContainsKey("grid"))
                grid = deserializeGrid(roamingSettings.Values["grid"].ToString());

            if (roamingSettings.Values.ContainsKey("firstPlayerTurn"))
                firstPlayerTurn = Convert.ToBoolean(roamingSettings.Values["firstPlayerTurn"].ToString());

            if (roamingSettings.Values.ContainsKey("topPlayers"))
                topPlayers = deserializePlayers(roamingSettings.Values["topPlayers"].ToString());

            if (roamingSettings.Values.ContainsKey("topPlayerScores"))
                topPlayerScores = deserializeScores(roamingSettings.Values["topPlayerScores"].ToString());

            populateTopPlayers();

            if (roamingSettings.Values.ContainsKey("firstPlayerColor"))
                firstPlayerColor = GetColorFromHexString(roamingSettings.Values["firstPlayerColor"].ToString());

            if (roamingSettings.Values.ContainsKey("secondPlayerColor"))
                secondPlayerColor = GetColorFromHexString(roamingSettings.Values["secondPlayerColor"].ToString());

            if (firstPlayerTurn)
            {
                interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
                interactionTextBlock.Text = firstPlayerName + "'s Turn!";
            }
            else
            {
                interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
                interactionTextBlock.Text = secondPlayerName + "'s Turn!";
            }
            if (playerWon(grid, 1))
            {
                interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
                interactionTextBlock.Text = firstPlayerName + " Won!";
            }
            if (playerWon(grid, 2))
            {
                interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
                interactionTextBlock.Text = secondPlayerName + " Won!";
            }

            DrawGrid();
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
             // Save session and app state
            Windows.Storage.ApplicationDataContainer roamingSettings =
        Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["firstPlayerName"] = firstPlayerName;

            roamingSettings.Values["secondPlayerName"] = secondPlayerName;

            roamingSettings.Values["firstPlayerScore"] = firstPlayerScore.ToString();

            roamingSettings.Values["secondPlayerScore"] = secondPlayerScore.ToString();

            roamingSettings.Values["grid"] = serializeGrid(grid);

            roamingSettings.Values["firstPlayerTurn"] = firstPlayerTurn.ToString();

            roamingSettings.Values["topPlayers"] = serializePlayers(topPlayers);

            roamingSettings.Values["topPlayerScores"] = serializeScores(topPlayerScores);

            roamingSettings.Values["secondPlayerColor"] = secondPlayerColor.ToString();

            roamingSettings.Values["firstPlayerColor"] = firstPlayerColor.ToString();

        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void DrawGrid()
        {
            int index = BOARD_HEIGHT * BOARD_WIDTH;

            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    Ellipse ellipse = boardCanvas.Children[index] as Ellipse;
                    index++;

                    ellipse.StrokeThickness = 3;
                    if (grid[r, c] == 1)
                    {
                        ellipse.Stroke = new SolidColorBrush(firstPlayerColor);
                    }
                    else if (grid[r, c] == 2)
                    {
                        ellipse.Stroke = new SolidColorBrush(secondPlayerColor);
                    }
                    else
                    {
                        ellipse.Stroke = new SolidColorBrush(Colors.Transparent);
                    }
                }
            }
        }

        private void newGame()
        {
            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    grid[r, c] = 0;
                }
            }

            interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
            interactionTextBlock.Text = firstPlayerName + "'s Turn!";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            firstPlayerTurn = true;
            newGame();
            DrawGrid();
        }


        private void columnClicked(object sender, PointerRoutedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            makePlay(Convert.ToInt32(textBlock.Name));
        }
        #endregion GUI

        private void Button_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            computerPlay();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DispatcherTimerSetup();
        }

        public void DispatcherTimerSetup()
        {
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {
            computerPlay();
            if (gameIsOver(grid))
            {
                timer.Stop();

            }
        }

        private int[,] copyGrid(int[,] grid)
        {
            int[,] result = { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    result[r, c] = grid[r, c];
                }
            }
            return result;
        }

        private void variableMoves(int[,] gameGrid, int column)
        {
            for (int r = BOARD_HEIGHT - 1; r > -1; r--)
            {
                if (gameGrid[r, column] == 0)
                {
                    if (firstPlayerTurn)
                    {
                        gameGrid[r, column] = 1;

                    }
                    if (!firstPlayerTurn)
                    {
                        gameGrid[r, column] = 2;

                    }
                    break;
                }
            }
        }

        public bool isEmpty(int[,] grid)
        {
            bool result = true;
            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    if(grid[r, c] != 0)
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        Play findBestPlay()
        {
            Play result = new Play(0,-10000);

            int[,] dummyGrid = copyGrid(grid);

            int score = -10000;
            int scoredMove = 0;

            if (grid[0, 0] == 0)
            {
                variableMoves(dummyGrid, 0);
                if (firstPlayerTurn)
                {
                    if (evaluateFor1(dummyGrid) > score)
                    {
                        score = evaluateFor1(dummyGrid);
                        scoredMove = 0;
                    }
                }
                else
                {
                    if (evaluateFor2(dummyGrid) > score)
                    {
                        score = evaluateFor2(dummyGrid);
                        scoredMove = 0;
                    }
                }
                dummyGrid = copyGrid(grid);
            }
            if (grid[0, 1] == 0)
            {
                variableMoves(dummyGrid, 1);
                if (firstPlayerTurn)
                {
                    if (evaluateFor1(dummyGrid) > score)
                    {
                        score = evaluateFor1(dummyGrid);
                        scoredMove = 1;
                    }
                }
                else
                {
                    if (evaluateFor2(dummyGrid) > score)
                    {
                        score = evaluateFor2(dummyGrid);
                        scoredMove = 1;
                    }
                }
                dummyGrid = copyGrid(grid);
            }
            if (grid[0, 2] == 0)
            {
                variableMoves(dummyGrid, 2);
                if (firstPlayerTurn)
                {
                    if (evaluateFor1(dummyGrid) > score)
                    {
                        score = evaluateFor1(dummyGrid);
                        scoredMove = 2;
                    }
                }
                else
                {
                    if (evaluateFor2(dummyGrid) > score)
                    {
                        score = evaluateFor2(dummyGrid);
                        scoredMove = 2;
                    }
                }
                dummyGrid = copyGrid(grid);
            }
            if (grid[0, 3] == 0)
            {
                variableMoves(dummyGrid, 3);
                if (firstPlayerTurn)
                {
                    if (evaluateFor1(dummyGrid) > score)
                    {
                        score = evaluateFor1(dummyGrid);
                        scoredMove = 3;
                    }
                }
                else
                {
                    if (evaluateFor2(dummyGrid) > score)
                    {
                        score = evaluateFor2(dummyGrid);
                        scoredMove = 3;
                    }
                }
                dummyGrid = copyGrid(grid);
            }
            if (grid[0, 4] == 0)
            {
                variableMoves(dummyGrid, 4);
                if (firstPlayerTurn)
                {
                    if (evaluateFor1(dummyGrid) > score)
                    {
                        score = evaluateFor1(dummyGrid);
                        scoredMove = 4;
                    }
                }
                else
                {
                    if (evaluateFor2(dummyGrid) > score)
                    {
                        score = evaluateFor2(dummyGrid);
                        scoredMove = 4;
                    }
                }
                dummyGrid = copyGrid(grid);
            }
            if (grid[0, 5] == 0)
            {
                variableMoves(dummyGrid, 5);
                if (firstPlayerTurn)
                {
                    if (evaluateFor1(dummyGrid) > score)
                    {
                        score = evaluateFor1(dummyGrid);
                        scoredMove = 5;
                    }
                }
                else
                {
                    if (evaluateFor2(dummyGrid) > score)
                    {
                        score = evaluateFor2(dummyGrid);
                        scoredMove = 5;
                    }
                }
                dummyGrid = copyGrid(grid);
            }

            result.column = scoredMove;
            result.score = score;

            return result;
        }

        //private Play returnBestPlay(Play zero, Play one, Play two, Play three, Play four, Play five)
        //{
        //    Play result = new Play(0, -100000);

        //    return result;
        //}

        struct playBoard
        {
            public int column;
            public int score;
            public int[,] playGrid;
            //public playBoard(int col, int sco)
            //{
            //    column = col;
            //    score = sco;
            //}
        }


        //check for full columns
        private playBoard lookAhead1(int[,] lookAheadGrid, int level, int end)
        {
            if(level == end)
            {
                if(level % 2 == 0)
                {
                    playBoard pboard = new playBoard();
                    pboard.column = 0;
                    pboard.score = -100000;
                    pboard.playGrid = new int[BOARD_HEIGHT,BOARD_WIDTH];
                    
                    for(int i = 0; i < BOARD_WIDTH; i++)
                    {
                        if (grid[0, i] == 0)
                        {
                            int[,] tempGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                            Array.Copy(lookAheadGrid, tempGrid, 42);
                            firstPlayerTurn = true;
                            variableMoves(tempGrid, i);
                            if (evaluateFor1(tempGrid) > pboard.score)
                            {
                                pboard.column = i;
                                pboard.score = evaluateFor1(tempGrid);
                                Array.Copy(tempGrid, pboard.playGrid, 42);
                            }
                        }
                    }
                    return pboard;
                }
                else
                {
                    playBoard pboard = new playBoard();
                    pboard.column = 0;
                    pboard.score = 100000;
                    pboard.playGrid = new int[BOARD_HEIGHT,BOARD_WIDTH];
                    
                    for (int i = 0; i < BOARD_WIDTH; i++)
                    {
                        if (grid[0, i] == 0)
                        {
                            int[,] tempGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                            Array.Copy(lookAheadGrid, tempGrid, 42);
                            firstPlayerTurn = false;
                            variableMoves(tempGrid, i);
                            if (evaluateFor1(tempGrid) < pboard.score)
                            {
                                pboard.column = i;
                                pboard.score = evaluateFor1(tempGrid);
                                Array.Copy(tempGrid, pboard.playGrid, 42);
                            }
                        }
                    }
                    return pboard;
                }
            }
            else if(level % 2 == 0)
            {
                playBoard pboard = new playBoard();
                pboard.column = 0;
                pboard.score = -100000;
                pboard.playGrid = new int[BOARD_HEIGHT,BOARD_WIDTH];
                
                level++;
                for(int i = 0; i < BOARD_WIDTH; i++)
                {
                    if (grid[0, i] == 0)
                    {
                        int[,] tempGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                        Array.Copy(lookAheadGrid, tempGrid, 42);
                        firstPlayerTurn = true;
                        variableMoves(tempGrid, i);
                        if (lookAhead1(tempGrid, level, end).score > pboard.score)
                        {
                            pboard = lookAhead1(tempGrid, level, end);
                            pboard.column = i;
                        }
                    }
                }
                return pboard;
            }
            else
            {
                playBoard pboard = new playBoard();
                pboard.column = 0;
                pboard.score = 100000;
                pboard.playGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                
                level++;
                for(int i = 0; i < BOARD_WIDTH; i++)
                {
                    if (grid[0, i] == 0)
                    {
                        int[,] tempGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                        Array.Copy(lookAheadGrid, tempGrid, 42);
                        firstPlayerTurn = false;
                        variableMoves(tempGrid, i);
                        if (lookAhead1(tempGrid, level, end).score < pboard.score)
                        {
                            pboard = lookAhead1(tempGrid, level, end);
                            pboard.column = i;
                        }
                    }
                }
                return pboard;
            }
        }

        private playBoard lookAhead2(int[,] lookAheadGrid, int level, int end)
        {
            if (level == end)
            {
                if (level % 2 == 0)
                {
                    playBoard pboard = new playBoard();
                    pboard.column = 0;
                    pboard.score = -100000;
                    pboard.playGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];

                    for (int i = 0; i < BOARD_WIDTH; i++)
                    {
                        if (grid[0, i] == 0)
                        {
                            int[,] tempGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                            Array.Copy(lookAheadGrid, tempGrid, 42);
                            firstPlayerTurn = false;
                            variableMoves(tempGrid, i);
                            if (evaluateFor2(tempGrid) > pboard.score)
                            {
                                pboard.column = i;
                                pboard.score = evaluateFor2(tempGrid);
                                Array.Copy(tempGrid, pboard.playGrid, 42);
                            }
                        }
                    }
                    return pboard;
                }
                else
                {
                    playBoard pboard = new playBoard();
                    pboard.column = 0;
                    pboard.score = 100000;
                    pboard.playGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];

                    for (int i = 0; i < BOARD_WIDTH; i++)
                    {
                        if (grid[0, i] == 0)
                        {
                            int[,] tempGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                            Array.Copy(lookAheadGrid, tempGrid, 42);
                            firstPlayerTurn = true;
                            variableMoves(tempGrid, i);
                            if (evaluateFor2(tempGrid) < pboard.score)
                            {
                                pboard.column = i;
                                pboard.score = evaluateFor2(tempGrid);
                                Array.Copy(tempGrid, pboard.playGrid, 42);
                            }
                        }
                    }
                    return pboard;
                }
            }
            else if (level % 2 == 0)
            {
                playBoard pboard = new playBoard();
                pboard.column = 0;
                pboard.score = -100000;
                pboard.playGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];

                level++;
                for (int i = 0; i < BOARD_WIDTH; i++)
                {
                    if (grid[0, i] == 0)
                    {
                        int[,] tempGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                        Array.Copy(lookAheadGrid, tempGrid, 42);
                        firstPlayerTurn = false;
                        variableMoves(tempGrid, i);
                        if (lookAhead2(tempGrid, level, end).score > pboard.score)
                        {
                            pboard = lookAhead2(tempGrid, level, end);
                            pboard.column = i;
                        }
                    }
                }
                return pboard;
            }
            else
            {
                playBoard pboard = new playBoard();
                pboard.column = 0;
                pboard.score = 100000;
                pboard.playGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];

                level++;
                for (int i = 0; i < BOARD_WIDTH; i++)
                {
                    if (grid[0, i] == 0)
                    {
                        int[,] tempGrid = new int[BOARD_HEIGHT, BOARD_WIDTH];
                        Array.Copy(lookAheadGrid, tempGrid, 42);
                        firstPlayerTurn = true;
                        variableMoves(tempGrid, i);
                        if (lookAhead2(tempGrid, level, end).score < pboard.score)
                        {
                            pboard = lookAhead2(tempGrid, level, end);
                            pboard.column = i;
                        }
                    }
                }
                return pboard;
            }
        }

        //private Play function(int levels, int finalLevel, int[,] gamegrid)
        //{
        //    if (levels == finalLevel)
        //    {
        //        Play p = new Play();
        //        p.score = evaluateFor1(gamegrid);
        //        //if (playerMove == 1)
        //        //{
        //        //    p.score = evaluateFor1(gamegrid);
        //        //}
        //        //else
        //        //{
        //        //    p.score = evaluateFor2(gamegrid);
        //        //}
        //        return p;
        //        //int min = 100000;
        //        //int[,] helperGrid = grid;
        //        //for (int i = 0; i < 6; i++)
        //        //{
        //        //    variableMoves(grid, i);
        //        //    //firstPlayerTurn = !firstPlayerTurn;
        //        //    if (evaluateFor1(grid) < min)
        //        //    {
        //        //        min = evaluateFor1(grid);
        //        //    }
        //        //    grid = helperGrid;
        //        //}
        //        //return min;
        //    }
        //    else if (levels % 2 == 0)
        //    {
        //        int[,] helperGrid = copyGrid(gamegrid);
        //        int max = -100000;
        //        Play x = new Play();
        //        if(levels != 0)
        //            firstPlayerTurn = !firstPlayerTurn;
        //        for (int i = 0; i < 6; i++)
        //        {
        //            variableMoves(helperGrid, i);
        //            //firstPlayerTurn = !firstPlayerTurn;
        //            x = function(levels + 1, finalLevel, helperGrid);
        //            if (x.score > max)
        //            {
        //                max = x.score;
        //                x.column = i;
        //            }
        //            helperGrid = copyGrid(gamegrid);
        //        }
        //        x.score = max;
        //        return x;
        //        //    //max of
        //        //    if (levels == finalLevel)
        //        //    {
        //        //        //max of heuristics
        //        //    }
        //        //return function(levels + 1,finalLevel);
        //    }
        //    else
        //    {
        //        int[,] helperGrid = copyGrid(gamegrid);
        //        int min = 100000;
        //        Play x = new Play();
        //        if (levels != 1)
        //            firstPlayerTurn = !firstPlayerTurn;
        //        for (int i = 0; i < 6; i++)
        //        {
        //            variableMoves(helperGrid, i);
        //            //firstPlayerTurn = !firstPlayerTurn;
        //            x = function(levels + 1, finalLevel, helperGrid);
        //            if (x.score < min)
        //            {
        //                min = x.score;
        //                x.column = i;
        //            }
        //            helperGrid = copyGrid(gamegrid);
        //        }
        //        x.score = min;
        //        return x;
        //        //return min;
        //        ////min
        //        //return function(levels + 1,finalLevel);
        //    }
        //}

        private void computerPlay()
        {
            Random rnd = new Random();
            int column = rnd.Next(0, BOARD_WIDTH);
            int player;

            if (firstPlayerTurn)
            {
                player = 1;
            }
            else
            {
                player = 2;
            }

            if (isEmpty(grid))
            {
                variableMoves(grid, column);
            }
            else
            {
                //variableMoves(grid, findBestPlay().column);

                //variableMoves(grid, function(0, 5, grid).column);

                bool turn = firstPlayerTurn;
                int move = 0;
                if (turn)
                {
                    move = lookAhead1(grid, 0, DEPTH).column;
                    move1.Text = move.ToString();
                    move1.Foreground = new SolidColorBrush(firstPlayerColor);
                }
                else
                {
                    move = lookAhead2(grid, 0, DEPTH).column;
                    move2.Text = move.ToString();
                    move2.Foreground = new SolidColorBrush(secondPlayerColor);
                }
                firstPlayerTurn = turn;
                variableMoves(grid, move);

            }

            firstPlayerTurn = !firstPlayerTurn;

            DrawGrid();
        }

        private async void makePlay(int column)
        {
            if (!gameIsOver(grid))
            {
                interactionTextBlock.Text = column.ToString();

                if (grid[0, column] != 0)
                {
                    interactionTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    interactionTextBlock.Text = "Invalid Move";
                }
                else
                {
                    for (int r = BOARD_HEIGHT - 1; r > -1; r--)
                    {
                        if (grid[r, column] == 0)
                        {
                            if (firstPlayerTurn)
                            {
                                grid[r, column] = 1;

                            }
                            if (!firstPlayerTurn)
                            {
                                grid[r, column] = 2;

                            }
                            break;
                        }
                    }
                    
                    firstPlayerTurn = !firstPlayerTurn;
                    DrawGrid();

                    if (!gameIsOver(grid))
                    {
                        interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
                        interactionTextBlock.Text = secondPlayerName + "'s Turn!";
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        bool turn = firstPlayerTurn;
                        int move = lookAhead2(grid, 0, DEPTH).column;
                        firstPlayerTurn = turn;
                        variableMoves(grid, move);

                        firstPlayerTurn = true;

                        DrawGrid();
                    }

                    if (firstPlayerTurn)
                    {
                        interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
                        interactionTextBlock.Text = firstPlayerName + "'s Turn!";
                    }
                    else
                    {
                        interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
                        interactionTextBlock.Text = secondPlayerName + "'s Turn!";
                    }
                    if (playerWon(grid, 1))
                    {
                        interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
                        interactionTextBlock.Text = firstPlayerName + " Won!";
                        firstPlayerScore++;
                        firstPlayerScoreTextBlock.Text = firstPlayerScore.ToString();

                        if (topPlayers.Contains(firstPlayerName))
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (topPlayers[i] == firstPlayerName)
                                {
                                    topPlayerScores[i] = firstPlayerScore;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (firstPlayerScore > topPlayerScores[i])
                                {
                                    for (int j = 4; j >= i + 1; j--)
                                    {
                                        topPlayerScores[j] = topPlayerScores[j - 1];
                                        topPlayers[j] = topPlayers[j - 1];
                                    }
                                    topPlayerScores[i] = firstPlayerScore;
                                    topPlayers[i] = firstPlayerName;
                                    break;
                                }
                            }
                        }
                        sortTopPlayers();
                        populateTopPlayers();
                    }
                    if (playerWon(grid, 2))
                    {
                        interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
                        interactionTextBlock.Text = secondPlayerName + " Won!";
                        secondPlayerScore++;
                        secondPlayerScoreTextBlock.Text = secondPlayerScore.ToString();

                        if (topPlayers.Contains(secondPlayerName))
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (topPlayers[i] == secondPlayerName)
                                {
                                    topPlayerScores[i] = secondPlayerScore;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (secondPlayerScore > topPlayerScores[i])
                                {
                                    for (int j = 4; j >= i + 1; j--)
                                    {
                                        topPlayerScores[j] = topPlayerScores[j - 1];
                                        topPlayers[j] = topPlayers[j - 1];
                                    }
                                    topPlayerScores[i] = secondPlayerScore;
                                    topPlayers[i] = secondPlayerName;
                                    break;
                                }
                            }
                        }
                        sortTopPlayers();
                        populateTopPlayers();
                    }
                }
                //if (firstPlayerTurn)
                //    garbage.Text = evaluateFor1(grid).ToString();
                //else
                //    garbage.Text = evaluateFor2(grid).ToString();
            }
        }
        //private void makePlay(int column)
        //{
        //    if (!gameIsOver(grid))
        //    {
        //        interactionTextBlock.Text = column.ToString();

        //        if (grid[0, column] != 0)
        //        {
        //            interactionTextBlock.Foreground = new SolidColorBrush(Colors.Red);
        //            interactionTextBlock.Text = "Invalid Move";
        //        }
        //        else
        //        {
        //            for (int r = BOARD_HEIGHT - 1; r > -1; r--)
        //            {
        //                if (grid[r, column] == 0)
        //                {
        //                    if (firstPlayerTurn)
        //                    {
        //                        grid[r, column] = 1;

        //                    }
        //                    if (!firstPlayerTurn)
        //                    {
        //                        grid[r, column] = 2;

        //                    }
        //                    break;
        //                }
        //            }
        //            firstPlayerTurn = !firstPlayerTurn;
        //            DrawGrid();
        //            if (firstPlayerTurn)
        //            {
        //                interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
        //                interactionTextBlock.Text = firstPlayerName + "'s Turn!";
        //            }
        //            else
        //            {
        //                interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
        //                interactionTextBlock.Text = secondPlayerName + "'s Turn!";
        //            }
        //            if (playerWon(grid,1))
        //            {
        //                interactionTextBlock.Foreground = new SolidColorBrush(firstPlayerColor);
        //                interactionTextBlock.Text = firstPlayerName + " Won!";
        //                firstPlayerScore++;
        //                firstPlayerScoreTextBlock.Text = firstPlayerScore.ToString();

        //                if (topPlayers.Contains(firstPlayerName))
        //                {
        //                    for(int i = 0; i < 5; i++)
        //                    {
        //                        if(topPlayers[i] == firstPlayerName)
        //                        {
        //                            topPlayerScores[i] = firstPlayerScore;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < 5; i++)
        //                    {
        //                        if (firstPlayerScore > topPlayerScores[i])
        //                        {
        //                            for (int j = 4; j >= i + 1; j--)
        //                            {
        //                                topPlayerScores[j] = topPlayerScores[j - 1];
        //                                topPlayers[j] = topPlayers[j - 1];
        //                            }
        //                            topPlayerScores[i] = firstPlayerScore;
        //                            topPlayers[i] = firstPlayerName;
        //                            break;
        //                        }
        //                    }
        //                }
        //                sortTopPlayers();
        //                populateTopPlayers();
        //            }
        //            if(playerWon(grid,2))
        //            {
        //                interactionTextBlock.Foreground = new SolidColorBrush(secondPlayerColor);
        //                interactionTextBlock.Text = secondPlayerName + " Won!";
        //                secondPlayerScore++;
        //                secondPlayerScoreTextBlock.Text = secondPlayerScore.ToString();

        //                if (topPlayers.Contains(secondPlayerName))
        //                {
        //                    for (int i = 0; i < 5; i++)
        //                    {
        //                        if (topPlayers[i] == secondPlayerName)
        //                        {
        //                            topPlayerScores[i] = secondPlayerScore;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < 5; i++)
        //                    {
        //                        if (secondPlayerScore > topPlayerScores[i])
        //                        {
        //                            for (int j = 4; j >= i + 1; j--)
        //                            {
        //                                topPlayerScores[j] = topPlayerScores[j - 1];
        //                                topPlayers[j] = topPlayers[j - 1];
        //                            }
        //                            topPlayerScores[i] = secondPlayerScore;
        //                            topPlayers[i] = secondPlayerName;
        //                            break;
        //                        }
        //                    }
        //                }
        //                sortTopPlayers();
        //                populateTopPlayers();
        //            }
        //        }
        //        //if (firstPlayerTurn)
        //        //    garbage.Text = evaluateFor1(grid).ToString();
        //        //else
        //        //    garbage.Text = evaluateFor2(grid).ToString();
        //    }
        //}
        #region More GUI
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage), null);
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Number1: makePlay(0); break;
                case VirtualKey.Number2: makePlay(1); break;
                case VirtualKey.Number3: makePlay(2); break;
                case VirtualKey.Number4: makePlay(3); break;
                case VirtualKey.Number5: makePlay(4); break;
                case VirtualKey.Number6: makePlay(5); break;
                case VirtualKey.N: newGame(); firstPlayerTurn = true; DrawGrid(); break;
            }
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditScoresPage), null);
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditPlayersPage), null);
        }

        private string serializeGrid(int[,] grid)
        {
            string result = "";
            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    result += grid[r, c].ToString();
                }
            }
            return result;
        }

        private int[,] deserializeGrid(string serialization)
        {
            int[,] result = { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    result[r, c] = Convert.ToInt32(serialization[0].ToString());
                    serialization = serialization.Substring(1);
                }
            }
            return result;
        }

        private string serializeScores(int[] scores)
        {
            string result = "";
            for (int c = 0; c < 5; c++)
            {
                result += scores[c].ToString() + ",";
            }
            return result;
        }

        private string serializePlayers(string[] players)
        {
            string result = "";
            for (int c = 0; c < 5; c++)
            {
                result += players[c] + ",";
            }
            return result;
        }

        private string[] deserializePlayers(string serialization)
        {
            return serialization.Split(',');
        }

        private int[] deserializeScores(string serialization)
        {
            int[] result = { 0, 0, 0, 0, 0 };
            string[] test = serialization.Split(',');
            for (int i = 0; i < 5; i++ )
            {
                result[i] = Convert.ToInt32(test[i]);
            }
            return result;
        }

        private string serializeColor(Color color)
        {
            return color.A.ToString() + color.R.ToString() + color.G.ToString() + color.B.ToString();
        }

        //https://social.msdn.microsoft.com/Forums/windowsapps/en-US/b639cd8a-30c2-48cf-99be-559f34cbfa79/convert-string-to-color-in-metro?forum=winappswithcsharp
        private Color GetColorFromHexString(string hexValue)
        {
            hexValue = hexValue.Substring(1);
            var a = Convert.ToByte(hexValue.Substring(0, 2), 16);
            var r = Convert.ToByte(hexValue.Substring(2, 2), 16);
            var g = Convert.ToByte(hexValue.Substring(4, 2), 16);
            var b = Convert.ToByte(hexValue.Substring(6, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }

        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {

        }

        private void populateTopPlayers()
        {
            topScorerTextBlock1.Text = topPlayers[0] + ":";
            topScoreTextBlock1.Text = "  " + topPlayerScores[0].ToString();

            topScorerTextBlock2.Text = topPlayers[1] + ":";
            topScoreTextBlock2.Text = "  " + topPlayerScores[1].ToString();

            topScorerTextBlock3.Text = topPlayers[2] + ":";
            topScoreTextBlock3.Text = "  " + topPlayerScores[2].ToString();

            topScorerTextBlock4.Text = topPlayers[3] + ":";
            topScoreTextBlock4.Text = "  " + topPlayerScores[3].ToString();

            topScorerTextBlock5.Text = topPlayers[4] + ":";
            topScoreTextBlock5.Text = "  " + topPlayerScores[4].ToString();
        }

        private void sortTopPlayers()
        {
            for(int i = 1; i < 5; i++)
            {
                while(topPlayerScores[i] > topPlayerScores[i-1])
                {
                    string testString = topPlayers[i - 1];
                    int testInt = topPlayerScores[i - 1];
                    topPlayers[i - 1] = topPlayers[i];
                    topPlayerScores[i - 1] = topPlayerScores[i];
                    topPlayers[i] = testString;
                    topPlayerScores[i] = testInt;
                }
            }
        }

        //private void updateScores()
        //{

        //}

        //private void populatePlayers()
        //{

        //}
        #endregion More GUI

        private bool gameIsOver(int[,] grid)
        {
            bool result = false;

            if (playerWon(grid, 1))
                result = true;
            if (playerWon(grid, 2))
                result = true;
            if (grid[0,0] != 0 && grid[0,1] != 0 && grid[0,2] != 0 && grid[0,3] != 0 && grid[0,4] != 0 && grid[0,5] != 0)
                result = true;
            
            return result;
        }

        private bool playerWon(int[,] grid, int player)
        {
            bool result = false;

            for (int r = 0; r < BOARD_HEIGHT; r++)
            {
                for (int c = 0; c < BOARD_WIDTH; c++)
                {
                    if(r <= BOARD_HEIGHT - 4)
                    {
                        if(grid[r,c] == player && grid[r+1,c] == player && grid[r+2,c] == player && grid[r+3,c] == player)
                            result = true;
                    }
                    if(c <= BOARD_WIDTH - 4)
                    {
                        if(grid[r,c] == player && grid[r,c + 1] == player && grid[r,c + 2] == player && grid[r,c + 3] == player)
                            result = true;
                    }
                    if (c <= BOARD_WIDTH - 4 && r <= BOARD_HEIGHT - 4)
                    {
                        if (grid[r, c] == player && grid[r + 1, c + 1] == player && grid[r + 2, c + 2] == player && grid[r + 3, c + 3] == player)
                            result = true;
                    }
                    if (c <= BOARD_WIDTH - 4 && r > 2)
                    {
                        if (grid[r, c] == player && grid[r - 1, c + 1] == player && grid[r - 2, c + 2] == player && grid[r - 3, c + 3] == player)
                            result = true;
                    }
                }
            }

            return result;
        }

        private int evaluateFor1(int[,] grid)
        {
            int result = 0;

            if (playerWon(grid, 1))
                result = FOUR_VALUE;
            if (playerWon(grid, 2))
                result = -FOUR_VALUE;


            if (!gameIsOver(grid))
            {
                for (int r = 0; r < BOARD_HEIGHT; r++)
                {
                    for (int c = 0; c < BOARD_WIDTH; c++)
                    {
                        if (r <= BOARD_HEIGHT - 3)
                        {
                            if (grid[r, c] == 1 && grid[r + 1, c] == 1 && grid[r + 2, c] == 1)
                                result += THREE_VALUE;
                            if (grid[r, c] == 2 && grid[r + 1, c] == 2 && grid[r + 2, c] == 2)
                                result -= THREE_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 3)
                        {
                            if (grid[r, c] == 1 && grid[r, c + 1] == 1 && grid[r, c + 2] == 1)
                                result += THREE_VALUE;
                            if (grid[r, c] == 2 && grid[r, c + 1] == 2 && grid[r, c + 2] == 2)
                                result -= THREE_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 3 && r <= BOARD_HEIGHT - 3)
                        {
                            if (grid[r, c] == 1 && grid[r + 1, c + 1] == 1 && grid[r + 2, c + 2] == 1)
                                result += THREE_VALUE;
                            if (grid[r, c] == 2 && grid[r + 1, c + 1] == 2 && grid[r + 2, c + 2] == 2)
                                result -= THREE_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 3 && r > 1)
                        {
                            if (grid[r, c] == 1 && grid[r - 1, c + 1] == 1 && grid[r - 2, c + 2] == 1)
                                result += THREE_VALUE;
                            if (grid[r, c] == 2 && grid[r - 1, c + 1] == 2 && grid[r - 2, c + 2] == 2)
                                result -= THREE_VALUE;
                        }
                    }
                }

                for (int r = 0; r < BOARD_HEIGHT; r++)
                {
                    for (int c = 0; c < BOARD_WIDTH; c++)
                    {
                        if (r <= BOARD_HEIGHT - 2)
                        {
                            if (grid[r, c] == 1 && grid[r + 1, c] == 1)
                                result += TWO_VALUE;
                            if (grid[r, c] == 2 && grid[r + 1, c] == 2)
                                result -= TWO_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 2)
                        {
                            if (grid[r, c] == 1 && grid[r, c + 1] == 1)
                                result += TWO_VALUE;
                            if (grid[r, c] == 2 && grid[r, c + 1] == 2)
                                result -= TWO_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 2 && r <= BOARD_HEIGHT - 2)
                        {
                            if (grid[r, c] == 1 && grid[r + 1, c + 1] == 1)
                                result += TWO_VALUE;
                            if (grid[r, c] == 2 && grid[r + 1, c + 1] == 2)
                                result -= TWO_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 2 && r > 0)
                        {
                            if (grid[r, c] == 1 && grid[r - 1, c + 1] == 1)
                                result += TWO_VALUE;
                            if (grid[r, c] == 2 && grid[r - 1, c + 1] == 2)
                                result += TWO_VALUE;
                        }
                    }
                }
            }

            return result;
        }

        private int evaluateFor2(int[,] grid)
        {
            int result = 0;

            if (playerWon(grid, 2))
                result = FOUR_VALUE;
            if (playerWon(grid, 1))
                result = -FOUR_VALUE;

            if (!gameIsOver(grid))
            {
                for (int r = 0; r < BOARD_HEIGHT; r++)
                {
                    for (int c = 0; c < BOARD_WIDTH; c++)
                    {
                        if (r <= BOARD_HEIGHT - 3)
                        {
                            if (grid[r, c] == 2 && grid[r + 1, c] == 2 && grid[r + 2, c] == 2)
                                result += THREE_VALUE;
                            if (grid[r, c] == 1 && grid[r + 1, c] == 1 && grid[r + 2, c] == 1)
                                result -= THREE_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 3)
                        {
                            if (grid[r, c] == 2 && grid[r, c + 1] == 2 && grid[r, c + 2] == 2)
                                result += THREE_VALUE;
                            if (grid[r, c] == 1 && grid[r, c + 1] == 1 && grid[r, c + 2] == 1)
                                result -= THREE_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 3 && r <= BOARD_HEIGHT - 3)
                        {
                            if (grid[r, c] == 2 && grid[r + 1, c + 1] == 2 && grid[r + 2, c + 2] == 2)
                                result += THREE_VALUE;
                            if (grid[r, c] == 1 && grid[r + 1, c + 1] == 1 && grid[r + 2, c + 2] == 1)
                                result -= THREE_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 3 && r > 1)
                        {
                            if (grid[r, c] == 2 && grid[r - 1, c + 1] == 2 && grid[r - 2, c + 2] == 2)
                                result += THREE_VALUE;
                            if (grid[r, c] == 1 && grid[r - 1, c + 1] == 1 && grid[r - 2, c + 2] == 1)
                                result -= THREE_VALUE;
                        }
                    }
                }

            
                for (int r = 0; r < BOARD_HEIGHT; r++)
                {
                    for (int c = 0; c < BOARD_WIDTH; c++)
                    {
                        if (r <= BOARD_HEIGHT - 2)
                        {
                            if (grid[r, c] == 2 && grid[r + 1, c] == 2)
                                result += TWO_VALUE;
                            if (grid[r, c] == 1 && grid[r + 1, c] == 1)
                                result -= TWO_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 2)
                        {
                            if (grid[r, c] == 2 && grid[r, c + 1] == 2)
                                result += TWO_VALUE;
                            if (grid[r, c] == 1 && grid[r, c + 1] == 1)
                                result -= TWO_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 2 && r <= BOARD_HEIGHT - 2)
                        {
                            if (grid[r, c] == 2 && grid[r + 1, c + 1] == 2)
                                result += TWO_VALUE;
                            if (grid[r, c] == 1 && grid[r + 1, c + 1] == 1)
                                result -= TWO_VALUE;
                        }
                        if (c <= BOARD_WIDTH - 2 && r > 0)
                        {
                            if (grid[r, c] == 2 && grid[r - 1, c + 1] == 2)
                                result += TWO_VALUE;
                            if (grid[r, c] == 1 && grid[r - 1, c + 1] == 1)
                                result -= TWO_VALUE;
                        }
                    }
                }
            }

            return result;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Play test = function(0, 7, grid);
            playBoard stuff = lookAhead1(grid, 0, 8);

            int something = 0;
        }
    }
}
