using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class TSPAgenda
    {
        private ArrayList[] _states;
        private int numStates;

        public TSPAgenda(int size)
        {
            _states = new ArrayList[size + 1];
            numStates = 0;
        }

        public bool hasState()
        {
            if (numStates > 0)
                return true;

            return false;
        }

        public TSPState getNextState()
        {
            //for(int i = _states.Length - 1; i >= 0; i--)
            //{
            //    ArrayList tempList = _states[i];
            //    if(tempList != null)
            //    {
            //        double minBound = double.MaxValue;
            //        int index = -1;
            //        for(int j = 0; j < tempList.Count; j++)
            //            if(((TSPState)tempList[j]).bound < minBound)
            //            {
            //                minBound = ((TSPState)tempList[j]).bound;
            //                index = j;
            //            }
            //        if(index != -1)
            //        {
            //            TSPState tempState = (TSPState)tempList[index];
            //            tempList.RemoveAt(index);
            //            numStates--;
            //            return tempState;
            //        }
            //    }
            //}
            return null;
        }

        public void addState(TSPState state)
        {
            //int index = state.pathSoFar.Count;

            //if (_states[index] == null)
            //    _states[index] = new ArrayList();

            //_states[index].Add(state);
            //numStates++;
        }

        public void pruneStates(double cost)
        {
            //for(int i = _states.Length - 1; i >= 0; i--)
            //{
            //    ArrayList tempList = _states[i];
            //    if (tempList != null)
            //    {
            //        for (int j = 0; j < tempList.Count; j++)
            //            if (((TSPState)tempList[j]).bound >= cost)
            //            {
            //                tempList.RemoveAt(j);
            //                numStates--;
            //            }
            //    }
            //}
        }
    }
}
