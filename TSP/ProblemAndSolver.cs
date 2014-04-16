using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

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
        private TSPSolution bssf; 

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
            bssf = null; 

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
            if (bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[bssf.Route.Count];
                int index = 0;
                foreach (City c in bssf.Route)
                {
                    if (index < bssf.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
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
            if (bssf != null)
                return (bssf.costOfRoute());
            else
                return -1D; 
        }

        /// <summary>
        ///  solve the problem.  This is the entry point for the solver when the run button is clicked
        /// right now it just picks a simple solution. 
        /// </summary>
        /// 
        PriorityQueue PQ;
        double BSSF;
        double currBound;
        List<int> BSSFList;
        double[] rowMins;

        public void solveProblem()
        {
            // Start off with some var power!
            Route = new ArrayList();
            double minCost;
            int minIndex = 0;
            int currIndex = 0;
            
            // Set our BSSF to 0, so we can create a new better one
            BSSFList = new List<int>();
            BSSFList.Add(0);

            // Begin with our first city
            Route.Add(Cities[currIndex]);

            // Use the nearest neighbor greedy algorithm
            // to find a random (naive) solution
            while(Route.Count < Cities.Length)
            {
                minCost = double.MaxValue;
                for(int j = 0; j < Cities.Length; j++)
                {
                    if(j != currIndex)
                        if(!Route.Contains(Cities[j]))
                        {
                            double currCost = Cities[currIndex].costToGetTo(Cities[j]);
                            if(currCost < minCost)
                            {
                                minCost = currCost;
                                minIndex = j;
                            }
                        }
                }
                // Update the BSSDlist and Route (creating BSSF)
                currIndex = minIndex;
                Route.Add(Cities[currIndex]);
                BSSFList.Add(currIndex);
            }
            // Save solution
            bssf = new TSPSolution(Route);
            BSSF = bssf.costOfRoute();

            //Build matrix for initial state
            double[,] initialMatrix = buildInitialMatrix();

            //Get the minimum cost for the remaining cities; kinda like bound 
            rowMins = getRowMins();
            
            //Generate list of children for initial state
            List<int> initStateChildren = new List<int>();
            for (int i = 1; i < Cities.Length; i++)
                initStateChildren.Add(i);
                                                         
            //Build initial state                                           
            TSPState initialState = new TSPState(0, initStateChildren, initialMatrix, new List<int>(), 0, 0, 0);
            initialState.Bound += boundingFunction(initialState);

            //Set the bound 
            currBound = initialState.Bound;

            //Start our PQ and load with init state
            PQ = new PriorityQueue();
            PQ.Enqueue(initialState, initialState.Bound);
            
            //Run Branch and Bound 
            branchAndBoundEngine();

        }

        DateTime endTime;
        
        public void branchAndBoundEngine()
        {
            // Start the stop watch
            DateTime startTime = DateTime.Now;
            endTime = startTime.AddSeconds(60);
            
            // Run until the PQ is empty, we find an optimal solution, or time runs out
            while(!PQ.IsEmpty() && DateTime.Now < endTime && BSSF > currBound)
            {
                // Get a state from the PQ
                TSPState state = PQ.Dequeue();
                // Check to see if the state is worth evaluating
                if(state.Bound < BSSF)
                {
                    // Generate the states children and iterate
                    List<TSPState> children = generateChildren(state);
                    foreach(TSPState child in children)
                    {
                        // If the bound is worth investigating...
                        if(child.Bound < bssf.costOfRoute())
                        {
                            // Check for a solution and save
                            if(child.IsSolution && child.Cost < BSSF)
                            {
                                // Save solution
                                BSSF = child.Cost;
                                BSSFList = child.PathSoFar;
                            }
                            // Otherwise assign the state's bound and Enqueue
                            else
                            {
                                double bound = child.Bound;
                                // Our bound of min cost path to destination + state bound
                                foreach (int childIndex in child.ChildList)
                                    bound += rowMins[childIndex];
                                PQ.Enqueue(child, bound);
                            }
                        }
                    }
                }
            }
            
            //
            // END BRANCH AND BOUND
            //  

            // Clear the route
            Route.Clear();
            // Save the BSSF route
            for (int i = 0; i < BSSFList.Count; i++)
                Route.Add(Cities[BSSFList[i]]);
            
            // Create our soltuion and assign
            bssf = new TSPSolution(Route);
            
            // Draw Results
            Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute();
            //Output the Time it took
            Program.MainForm.tbElapsedTime.Text = " " + (DateTime.Now - startTime);
            // do a refresh. 
            Program.MainForm.Invalidate();
        }

        public List<TSPState> generateChildren(TSPState state)
        {
            // Create new state list
            List<TSPState> children = new List<TSPState>();
            // Iterate through the current child's children
            foreach(int child in state.ChildList)
            {
                // Copy values from parent state so we can modify
                List<int> childList = new List<int>(state.ChildList);
                List<int> pathSoFar = new List<int>(state.PathSoFar);
                double cost = Cities[state.City].costToGetTo(Cities[child]);
                double[,] matrix = (double[,])state.Matrix.Clone();
                
                // Remove child from child list
                childList.Remove(child);
                // Add the parent state city to the path so far
                pathSoFar.Add(state.City);

                // Reduce the matrix
                for (int j = 0; j <= matrix.GetUpperBound(0); j++)
                    matrix[j, state.City] = double.MaxValue;
                
                // Create a new state
                TSPState newState = new TSPState(state.Bound + state.Matrix[state.City, child], childList, matrix, pathSoFar, state.Cost + cost, child, state.TreeDepth + 1);
                // Update the bound
                newState.Bound += boundingFunction(newState);
                
                // Check for a soltuion
                if(newState.IsSolution)
                {
                    // Mark state as a solution
                    newState.Cost += Cities[newState.City].costToGetTo(Cities[0]);
                    newState.PathSoFar.Add(newState.City);
                }
                
                // Add child to childrens state
                children.Add(newState);
            }
            
            // Returnt the list for later usage
            return children;
        }

        public double[,] buildInitialMatrix()
        {
            // Create a matrix
            double[,] matrix = new double[Cities.Length, Cities.Length];
            for(int i = 0; i < Cities.Length; i++)
            {
                for(int j = 0; j < Cities.Length; j++)
                {
                    if (i == j)
                        // Assign infinity if i == j
                        matrix[i, j] = double.MaxValue;
                    else
                        // Otherwise populate the matrix with real numbers
                        matrix[i, j] = Cities[i].costToGetTo(Cities[j]);
                }
            }
            return matrix;
        }

        public double[] getRowMins()
        {
            // Create an array getting the min cost from cities
            double[] rowMins = new double[Cities.Length];
            for (int i = 0; i < Cities.Length; i++)
            {
                double rowMin = double.MaxValue;
                for(int j = 0; j < Cities.Length; j++)
                {
                    if(i != j)
                    {
                        double currCost = Cities[i].costToGetTo(Cities[j]);
                        if (currCost < rowMin)
                            rowMin = currCost;
                    }
                }
                rowMins[i] = rowMin;
            }

            return rowMins;
        }

        public double boundingFunction(TSPState state)
        {
            // Start with 0 vals and the state's matrix
            double bound = 0;
            double numRows = 0;
            double[,] matrix = state.Matrix;

            // Reduce the matrix rows
            // Create a child city list
            List<int> childList = new List<int>(state.ChildList);
            // Add the current city to the list, creating all cities
            childList.Add(state.City);

            // Iterate through state's 1D col or row
            for (int i = 0; i < childList.Count; i++)
            {
                // Look for the min cost
                double minCost = double.MaxValue;
                // Interage through the other state's 1D col or row
                for(int j = 0; j < state.ChildList.Count; j++)
                {
                    // Check if cost is less than min...
                    if(matrix[childList[i], state.ChildList[j]] < minCost)
                    {
                        // Update the min cost
                        minCost = matrix[childList[i], state.ChildList[j]];
                    }
                }
                
                // Then once you find min cost and it's not infinity...
                if (minCost < double.MaxValue)
                {
                    // Reduce the matrix
                    for (int j = 0; j < state.ChildList.Count; j++)
                        matrix[childList[i], state.ChildList[j]] -= minCost;
                    // Add the cost to the bound
                    bound += minCost;
                }
                else
                    // Mark as infinity
                    numRows++;
            }

            // Reduce the matrix columns
            for (int i = 0; i < state.ChildList.Count; i++)
            {
                // Look for the min cost
                double minCost = double.MaxValue;
                // Iterate through each column
                for(int j = 0; j < childList.Count; j++)
                {
                    // Update min cost if the cell's value is less
                    if (matrix[childList[j], state.ChildList[i]] < minCost)
                        minCost = matrix[childList[j], state.ChildList[i]];
                }
                // Now that we have min cost, see if it is less than infinity
                if (minCost < double.MaxValue)
                    // If so, reduce the matrix column
                    for (int j = 0; j < childList.Count; j++)
                        matrix[childList[j], state.ChildList[i]] -= minCost;
                else
                    // Mark as infinity
                    numRows++;
            }
            
            // If entire matrix is infinity
            if(numRows >= matrix.GetUpperBound(0))
            {
                // Save solution
                state.IsSolution = true;
                state.Cost += Cities[1].costToGetTo(Cities[state.City]);
            }
            // Return 0 if it is a solution
            if (state.IsSolution)
                return 0;
            
            // Otherwise return the bound as normal
            return bound;
        }

        #endregion
    }
}
