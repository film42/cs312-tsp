using System;
using System.Collections.Generic;
using System.Text;

namespace TSP
{
    class TSPState
    {
        private double bound;
        private List<int> childList;
        private double[,] matrix;
        private List<int> pathSoFar;
        private double cost;
        private int city;
        private int treeDepth;
        private bool isSolution;

        public TSPState(double Bound, List <int> ChildList, double[,] Matrix, List<int> PathSoFar,
                        double Cost, int City, int TreeDepth)
        {
            bound = Bound;
            childList = ChildList;
            matrix = Matrix;
            pathSoFar = PathSoFar;
            cost = Cost;
            city = City;
            treeDepth = TreeDepth;

            if (childList.Count == 0)
                isSolution = true;
            else
                isSolution = false;

            
        }

        public double Bound
        {
            get { return bound; }
            set { bound = value; }
        }

        public List<int> ChildList
        {
            get { return childList; }
            set { childList = value; }
        }

        public double[,] Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }

        public List<int> PathSoFar
        {
            get { return pathSoFar; }
            set { pathSoFar = value; }
        }

        public double Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        public int City
        {
            get { return city; }
            set { city = value; }
        }

        public int TreeDepth
        {
            get { return treeDepth; }
            set { treeDepth = value; }
        }

        public Boolean IsSolution
        {
            get { return isSolution; }
            set { isSolution = value; }
        }
    }
}
