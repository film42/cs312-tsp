using System;
using System.Collections.Generic;
using System.Text;

namespace TSP
{
    class TSPState
    {
        double[,] _matrix;
        private double _cost;
        private double _bound;
        private int _depth;
        private List<TSPState> _children;
        private List<int> _pathSoFar;
        private int _cityIndex;

        public TSPState(double[,] matrix, double bound, double cost, int depth, List<int> pathSoFar, int cityIndex)
        {
            _matrix = matrix;
            _bound = bound;
            _cost = cost;
            _depth = depth;
            _pathSoFar = pathSoFar;
            _cityIndex = cityIndex;

            // Setup state class
            _children = new List<TSPState>();
        }

        public double Bound
        {
            get { return _bound; }
        }

        public int Depth
        {
            get { return _depth; }
        }

        public int City
        {
            get { return _cityIndex; }
        }

        public List<TSPState> Children
        {
            get { return _children; }
        }

        public double Cost
        {
            get { return _cost; }
        }

        public List<int> PathSoFar
        {
            get { return _pathSoFar; }
        }

        public double[,] Matrix
        {
            get { return _matrix; }
        }

        public double Priority()
        {
            return Cost + Bound;
        }

        public bool IsPathSoFar(int childCity)
        {
            return PathSoFar.Contains(childCity);
        }

        public void AddChild(TSPState state)
        {
            Children.Add(state);
        }

    }
}
