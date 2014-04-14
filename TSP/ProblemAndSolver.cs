using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

namespace TSP
{
    class ProblemAndSolver
    {
        private class TSPSolution
        {
            /// <summary>
            /// we use the representation [cityB,cityA,cityC] 
            /// to mean that cityB is the first city in the solution, cityA is the second, cityC is the third 
            /// and the edge from cityC to cityB is the final edge in the path.  
            /// you are, of course, free to use a different representation if it would be more convenient or efficient 
            /// for your node data structure and search algorithm. 
            /// </summary>
            public ArrayList 
                Route;

            public TSPSolution(ArrayList iroute)
            {
                Route = new ArrayList(iroute);
            }


            /// <summary>
            ///  compute the cost of the current route.  does not check that the route is complete, btw.
            /// assumes that the route passes from the last city back to the first city. 
            /// </summary>
            /// <returns></returns>
            public double costOfRoute()
            {
                // go through each edge in the route and add up the cost. 
                int x;
                City here; 
                double cost = 0D;
                
                for (x = 0; x < Route.Count-1; x++)
                {
                    here = Route[x] as City;
                    cost += here.costToGetTo(Route[x + 1] as City);
                }
                // go from the last city to the first. 
                here = Route[Route.Count - 1] as City;
                cost += here.costToGetTo(Route[0] as City);
                return cost; 
            }
        }

        #region private members
        private const int DEFAULT_SIZE = 25;
        
        private const int CITY_ICON_SIZE = 5;

        /// <summary>
        /// the cities in the current problem.
        /// </summary>
        private City[] Cities;
        /// <summary>
        /// a route through the current problem, useful as a temporary variable. 
        /// </summary>
        private ArrayList Route;
        /// <summary>
        /// best solution so far. 
        /// </summary>
        private TSPSolution BSSF; 

        /// <summary>
        /// how to color various things. 
        /// </summary>
        private Brush cityBrushStartStyle;
        private Brush cityBrushStyle;
        private Pen routePenStyle;


        /// <summary>
        /// keep track of the seed value so that the same sequence of problems can be 
        /// regenerated next time the generator is run. 
        /// </summary>
        private int _seed;
        /// <summary>
        /// number of cities to include in a problem. 
        /// </summary>
        private int _size;

        /// <summary>
        /// random number generator. 
        /// </summary>
        private Random rnd;
        #endregion

        #region public members.
        public int Size
        {
            get { return _size; }
        }

        public int Seed
        {
            get { return _seed; }
        }
        #endregion

        public const int DEFAULT_SEED = -1;

        #region Constructors
        public ProblemAndSolver()
        {
            initialize(DEFAULT_SEED, DEFAULT_SIZE);
        }

        public ProblemAndSolver(int seed)
        {
            initialize(seed, DEFAULT_SIZE);
        }

        public ProblemAndSolver(int seed, int size)
        {
            initialize(seed, size);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// reset the problem instance. 
        /// </summary>
        private void resetData()
        {
            Cities = new City[_size];
            Route = new ArrayList(_size);
            BSSF = null; 

            for (int i = 0; i < _size; i++)
                Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble());

            cityBrushStyle = new SolidBrush(Color.Black);
            cityBrushStartStyle = new SolidBrush(Color.Red);
            routePenStyle = new Pen(Color.LightGray,1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        private void initialize(int seed, int size)
        {
            this._seed = seed;
            this._size = size;
            if (seed != DEFAULT_SEED)
                this.rnd = new Random(seed);
            else
                this.rnd = new Random();
            this.resetData();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// make a new problem with the given size.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size)
        {
            this._size = size;
            resetData();
        }

        /// <summary>
        /// return a copy of the cities in this problem. 
        /// </summary>
        /// <returns>array of cities</returns>
        public City[] GetCities()
        {
            City[] retCities = new City[Cities.Length];
            Array.Copy(Cities, retCities, Cities.Length);
            return retCities;
        }

        /// <summary>
        /// draw the cities in the problem.  if the bssf member is defined, then
        /// draw that too. 
        /// </summary>
        /// <param name="g">where to draw the stuff</param>
        public void Draw(Graphics g)
        {
            float width  = g.VisibleClipBounds.Width-45F;
            float height = g.VisibleClipBounds.Height-15F;
            Font labelFont = new Font("Arial", 10);

            g.DrawString("n(c) means this node is the nth node in the current solution and incurs cost c to travel to the next node.", labelFont, cityBrushStartStyle, new PointF(0F, 0F)); 

            // Draw lines
            if (BSSF != null)
            {
                // make a list of points. 
                Point[] ps = new Point[BSSF.Route.Count];
                int index = 0;
                foreach (City c in BSSF.Route)
                {
                    if (index < BSSF.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(BSSF.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(BSSF.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    ps[index++] = new Point((int)(c.X * width) + CITY_ICON_SIZE / 2, (int)(c.Y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)Cities[0].X * width - 1, (float)Cities[0].Y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in Cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.X * width, (float)c.Y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }

        }

        /// <summary>
        ///  return the cost of the best solution so far. 
        /// </summary>
        /// <returns></returns>
        public double costOfBssf ()
        {
            if (BSSF != null)
                return (BSSF.costOfRoute());
            else
                return -1D; 
        }

        /// <summary>
        ///  solve the problem.  This is the entry point for the solver when the run button is clicked
        /// right now it just picks a simple solution. 
        /// </summary>
        /// 
        private System.Timers.Timer sixtySecondTimer;
        private Thread tspThread;
        private DateTime endTime;
        public void solveProblem()
        {
            int x;
            Route = new ArrayList();
            // this is the trivial solution. 
            for (x = 0; x < Cities.Length; x++)
            {
                Route.Add(Cities[Cities.Length - x - 1]);
            }
            //// call this the best solution so far.  bssf is the route that will be drawn by the Draw method. 
            BSSF = new TSPSolution(Route);
            // update the cost of the tour. 
            Program.MainForm.tbCostOfTour.Text = " " + BSSF.costOfRoute();
            Console.WriteLine("Initial BSSF: " + BSSF.costOfRoute());
            // do a refresh. 
            Program.MainForm.Invalidate();

            //Generate initial cost matrix before we begin the timers
            var matrix = BuildCostMatrix();
//Test BuildCostMatrix
            //for (int i = 0; i < Cities.Length; i++)
            //{
            //    for (int j = 0; j < Cities.Length; j++)
            //        Console.Write(String.Format("{0}\t", matrix[i, j]));
            //    Console.WriteLine();
            //}



            //Generate initial state
            double bound = BoundaryFunction(matrix, 0);
            double cost = 0.0;
            int depth = 0;
            int cityIndex = 0;
            var pathSoFar = new List<int>();
            pathSoFar.Add(0); // Add first city index
            var initState = new TSPState(matrix, bound, cost, depth, pathSoFar, cityIndex);

            // Enqueue
            AddToAgenda(initState);

            //Implement Timer
            //sixtySecondTimer = new System.Timers.Timer(60000);
            //sixtySecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(timeout);
            //sixtySecondTimer.AutoReset = false;

            ////Start a new thread
            //tspThread = new Thread(new ThreadStart(BranchAndBoundEngine));
            //tspThread.Start();
            //while (!tspThread.IsAlive) ;
            //sixtySecondTimer.Enabled = true;
            DateTime startTime = DateTime.Now;
            endTime = startTime.AddSeconds(60);
            BranchAndBoundEngine(bound);
            //if(sixtySecondTimer.Enabled)
            //{
            //    //Optimal solution found in less than sixty seconds
            //}
            //else
            //{
            //    //Optimal solution not found, return BSSF
            //}
            
            //tspThread.Join();

            // update the cost of the tour. 
            Program.MainForm.tbCostOfTour.Text = " (Ours) " + BSSF.costOfRoute();
            Program.MainForm.tbElapsedTime.Text = " (Ours) " + (DateTime.Now - startTime);
            // do a refresh. 
            Program.MainForm.Invalidate();
        }

        private void BranchAndBoundEngine(double startBound)
        {
            while (!PQ.IsEmpty && BSSF.costOfRoute() != startBound) {
                TSPState state = PQ.Dequeue();

                // Now we're done
                if (state.Cost >= BSSF.costOfRoute())
                    break;

                // Test for solution
                if (IsSolution(state)) {
                    SaveSolution(state);
                    PQ.Prune(state.Priority());
                }
                // Otherwise we generate states
                else
                {
                    // This is state expansion
                    StateExpansion(state);
                }
            }
            if (PQ.IsEmpty)
                Console.WriteLine("Empty Queue");
            else
                Console.WriteLine("BSSF = startBound");
        }

        private void SaveSolution(TSPState state)
        {
            // Potential Solution
            for (int i = 0; i < state.PathSoFar.Count; i++)
            {
                // Replace the route with the BSSF
                Route[i] = Cities[state.PathSoFar[i]];
            }

            // Apply the new BSSF
            BSSF = new TSPSolution(Route);
            Console.WriteLine(BSSF.costOfRoute());
        }

        private void timeout(object o, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Time is up!");
            tspThread.Abort();
        }


        private double[,] BuildCostMatrix()
        {
            int matrixSize = Cities.Length;
            var matrix = new double[matrixSize, matrixSize];
            // Iterate through graph; assuming totally connected
            for (int i = 0; i < Cities.Length; i++)
                for (int j = 0; j < Cities.Length; j++ )
                {
                    // If equal mark as infinity
                    if (i == j)
                        matrix[i, j] = double.MaxValue;
                    // Otherwise mark real cost
                    else
                        matrix[i, j] = Cities[i].costToGetTo(Cities[j]);
                }
            // Matrix is now built; return.
            return matrix;
        }


        #endregion


        #region Branch and Bound
        private PriorityQueue PQ = new PriorityQueue();
        //private AltPQ<double, TSPState> PQ = new AltPQ<double, TSPState>();

        private bool InAgenda(TSPState state) {
          //double prio = state.Priority();
          //var kvp = new KeyValuePair<double, TSPState>(prio, state);
          //return PQ.Contains(kvp);
          return PQ.Contains(state);
        }

        private void AddToAgenda(TSPState state) {
          //PQ.Enqueue(state.Priority(), state);
          PQ.Enqueue(state);
        }


        public void StateExpansion(TSPState state) 
        {
            // Generate Children of the first state
            GenerateChildren(state);

            // Iterate
            foreach (TSPState child in state.Children)
            {
                if (!state.IsPathSoFar(child.City) && !InAgenda(child))
                {
                    // Enqueue will look for priority
                    AddToAgenda(child);
                }
                else
                {
                    // Optimize: Cache the children we no longer want to use.
                    // We should some how hash them so we get quick lookup times.
                    // WE ARE NOT USING THIS SO DELETE
                    // Console.WriteLine("Child not added");
                }
            }
        }

        private void PrintMatrix(double[,] matrix)
        {
            Console.WriteLine("************");
            for (int k = 0; k < Cities.Length; k++)
            {
                for (int j = 0; j < Cities.Length; j++)
                    if(matrix[k,j] < double.MaxValue)
                        Console.Write(String.Format("{0}\t\t\t\t\t\t\t", matrix[k, j]));
                    else
                        Console.Write(String.Format("{0}\t\t", matrix[k, j]));

                Console.WriteLine();
            }
        }

        public void GenerateChildren(TSPState state)
        {
            // Iterate through the city index row
            for (int i = 0; i < Cities.Length; i++)
            {
                // Create child if not infinity
                if (state.Matrix[state.City, i] < double.MaxValue)
                {
                    // 1) already generated path so far
                    //

                    // Ensure the cost is less than BSSF
                    double costToChildI = Cities[state.City].costToGetTo(Cities[i]) + state.Cost;
                    if(costToChildI < BSSF.costOfRoute()) 
                    {
                        // Start with the existing Matrix
                        var matrix = CopyMatrix(state.Matrix);
                        //Generate initial state
                        double cost = costToChildI;
                        int depth = state.Depth + 1;
                        int cityIndex = i;
                        var pathSoFar = GenerateNewPathSoFar(state.PathSoFar, i);
//Print matrix for comparison
                        //PrintMatrix(matrix); // DEBUG

                        // Now reduce the matrix
                        // 1) Remove child from matrix: set pos index to infinity and the cols and rows
                        matrix[state.City, i] = double.MaxValue;
                        // Set inverse to infinity
                        matrix[i, state.City] = double.MaxValue;
                        // Set Col and Inverse row to infinity
                        for (int row = 0; row < Cities.Length; row++)
                        {
                            // Main row
                            matrix[row, i] = double.MaxValue;
                            // Parents Row
                            matrix[state.City, row] = double.MaxValue;
                        }
                        

//Test the removal of a child from the matrix
                        //PrintMatrix(matrix); // DEBUG


                        // 2) Then just reduce minrow and mincol
                        double bound = BoundaryFunction(matrix, state.Bound);
                        

                       
                        //PrintMatrix(matrix); // DEBUG
                        //Console.WriteLine("####################################");

                        var newChild = new TSPState(matrix, bound, cost, depth, pathSoFar, cityIndex);

                        // Add a child to state
                        state.AddChild(newChild);
                    }
                }
            }
        }

        private double[,] CopyMatrix(double[,] old)
        {
            double[,] newMatrix = new double[Cities.Length, Cities.Length];

            // Copy the old matrix into the new one
            Array.Copy(old, 0, newMatrix, 0, old.Length);

            return newMatrix;
        }

        private List<int> GenerateNewPathSoFar(List<int> oldPSF, int newIndex)
        {
            // TODO: Is this not a bug?
            var psf = new List<int>(oldPSF);
            psf.Add(newIndex);
            return psf;
        }

        private int GetMinColIndex(double[,] matrix, int rowIndex) 
        {
            double minValue = double.MaxValue;
            int minIndex = -1;
            for (int col = 0; col < matrix.GetLength(0); col++)
                if (matrix[rowIndex, col] < minValue)
                {
                    minValue = matrix[rowIndex, col];
                    minIndex = col;
                }

            return minIndex;
        }

        private int GetMinRowIndex(double[,] matrix, int colIndex)
        {
            double minValue = double.MaxValue;
            int minRowIndex = -1;
            for (int row = 0; row < matrix.GetLength(0); row++)
                if (matrix[row, colIndex] < minValue)
                {
                    minValue = matrix[row, colIndex];
                    minRowIndex = row;
                }

            return minRowIndex;
        }

        private double BoundaryFunction(double[,] matrix, double bound)
        {
            // 1) Reduce the cost Matrix for the state
            //    - Skip the columns of the cities that are present in the PathSoFar
            //    - As you reduce, add to the Bound
            for (int row = 0; row < Cities.Length; row++)
            {
                // Find the smallest bound
                int minColIndex = GetMinColIndex(matrix, row);
                double minValue;
                if(minColIndex != -1)
                    minValue = matrix[row, minColIndex];
                // This row has no eligible cities
                else continue;

                // If the column is not all infinity, add the min cost
                if (minValue < double.MaxValue)
                {
                    bound += minValue;
                    matrix = ReduceCostMatrixRow(matrix, row, minColIndex);
                }
            }
/////////////Test Row Reduction
            //Console.WriteLine("********************");
            //for (int i = 0; i < matrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < matrix.GetLength(1); j++)
            //        Console.Write(String.Format("{0}\t\t\t\t\t\t\t", matrix[i, j]));

            //    Console.WriteLine();
            //}
            // TODO: MAKE SURE THIS ISNT TERRIBLE
            // If columns are not reduced, reduce further
            //if (!IsMatrixReduced(matrix))
            //{
                for (int col = 0; col < Cities.Length; col++)
                {

                    // Find the smallest bound
                    int minRowIndex = GetMinRowIndex(matrix, col);
                    double minValue;
                    if (minRowIndex != -1)
                        minValue = matrix[minRowIndex, col];
                    // This row has no eligible cities
                    else continue;

                    // If the column is not all infinity, add the min cost
                    if (minValue < double.MaxValue)
                    {
                        bound += minValue;
                        matrix = ReduceCostMatrixCol(matrix, minRowIndex, col);
                    }
                }
            // }
//////////////Test Column Reduction
            //for (int i = 0; i < matrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < matrix.GetLength(1); j++)
            //        Console.Write(String.Format("{0}\t\t\t\t\t\t\t", matrix[i, j]));

            //    Console.WriteLine();
            //}
                    // Now it's finally reduced; return
                    return bound;
        }

        private double[,] ReduceCostMatrixRow(double[,] matrix, int row, int col) {
            int matrixSize = Cities.Length;
            //var reducedMatrix = new double[matrixSize, matrixSize];
            //Console.WriteLine("Row: " + row + "     " + "Col: " + col);

            double minValue = matrix[row, col];
            for (int i = 0; i < matrixSize; i++)
            {
                if (matrix[row, i] < double.MaxValue)
                    matrix[row, i] -= minValue;

                if (matrix[row, i] < 0)
                {
                    Console.WriteLine();
                }
            }

            // Return reduced matrix
            return matrix;
        }

        private double[,] ReduceCostMatrixCol(double[,] matrix, int row, int col)
        {
            int matrixSize = Cities.Length;
            //var reducedMatrix = new double[matrixSize, matrixSize];
            //Console.WriteLine("Row: " + col + "     " + "Col: " + row);

            double minValue = matrix[row, col];
            for (int i = 0; i < matrixSize; i++)
            {
                if (matrix[i, col] < double.MaxValue)
                    matrix[i, col] -= minValue;

                if (matrix[i, col] < 0)
                {
                    Console.WriteLine();
                }
            }

            // Return reduced matrix
            return matrix;
        }

        private bool IsMatrixReduced(double[,] matrix) 
        {
            // Look for a zero in each col
            for (int i = 0; i < Cities.Length; i++)
            {
                // Reset the zero check
                bool foundZero = false;
                for (int j = 0; j < Cities.Length; j++)
                {
                    // Check COLS then check ROWS; Confusing logic Warning!
                    if (matrix[j, i] == 0)
                        foundZero = true;
                }
                // Stop instantly if 0 was not found in a Col
                if (!foundZero) return false;
            }

            return true;
        }

        private bool IsSolution(TSPState state)
        {
 
            if (state.PathSoFar.Count == Cities.Length) 
            {
                double totalCost = Cities[state.PathSoFar[Cities.Length - 1]].costToGetTo(Cities[state.PathSoFar[0]])
                                    + state.Cost;

                // Return true if cost is less
                return totalCost < BSSF.costOfRoute();
            }

            return false;
        }
        #endregion
    }
}
