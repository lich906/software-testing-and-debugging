using System;
using System.Collections.Generic;

public interface IEquationSolverUiObserver
{
    void OnCoeffChanged();

    void OnNoRootsEvaluated();

    void OnSingleRootEvaluated();

    void OnDoubleRootsEvaluated();
}

namespace VisualEquationSolver
{
    using Token = Int32;
    public class EquationSolver
    {
        private List<IEquationSolverUiObserver> _observers = new List<IEquationSolverUiObserver>();

        private List<double> _roots = new List<double>();

        private bool _coeffChanged = false;

        public EquationSolver()
        {
            QuadraticCoeff = 0;
            LinearCoeff = 0;
            ConstantCoeff = 0;
        }

        public double QuadraticCoeff { get; private set; }
        public double LinearCoeff { get; private set; }
        public double ConstantCoeff { get; private set; }

        public void SetQuadraticCoeff(double value)
        {
            if (value != QuadraticCoeff)
            {
                QuadraticCoeff = value;
                NotifyCoeffChanged();
            }
        }

        public void SetLinearCoeff(double value)
        {
            if (value != LinearCoeff)
            {
                LinearCoeff = value;
                NotifyCoeffChanged();
            }
        }

        public void SetConstantCoeff(double value)
        {
            if (value != ConstantCoeff)
            {
                ConstantCoeff = value;
                NotifyCoeffChanged();
            }
        }

        public List<double> GetRoots()
        {
            if (QuadraticCoeff == 0)
            {
                throw new InvalidOperationException("Quadratic coeff equals zero");
            }

            if (_coeffChanged)
            {
                EvaluateRoots();
                _coeffChanged = false;
            }

            return _roots;
        }

        public Token Subscribe(IEquationSolverUiObserver observer)
        {
            _observers.Add(observer);

            return _observers.Count - 1;
        }

        public void Unsubscribe(Token token)
        {
            _observers.RemoveAt(token);
        }

        private void EvaluateRoots()
        {
            _roots.Clear();

            var d = GetDiscriminant();
            if (d == 0)
            {
                _roots = new List<double>
                {
                    - LinearCoeff / (2 * QuadraticCoeff)
                };
                NotifySingleRootEvaluated();
            }
            else if (d > 0)
            {
                _roots = new List<double>
                {
                    - (LinearCoeff - Math.Sqrt(d)) / (2 * QuadraticCoeff),
                    - (LinearCoeff + Math.Sqrt(d)) / (2 * QuadraticCoeff),
                };
                NotifyDoubleRootsEvaluated();
            }
            else
            {
                NotifyNoRootsEvaluated();
            }
        }

        private double GetDiscriminant()
        {
            return LinearCoeff * LinearCoeff - 4 * QuadraticCoeff * ConstantCoeff;
        }

        private void NotifyCoeffChanged()
        {
            _coeffChanged = true;

            _observers.ForEach(o => o.OnCoeffChanged());
        }

        private void NotifyNoRootsEvaluated()
        {
            _observers.ForEach(o => o.OnNoRootsEvaluated());
        }
        private void NotifySingleRootEvaluated()
        {
            _observers.ForEach(o => o.OnSingleRootEvaluated());
        }
        private void NotifyDoubleRootsEvaluated()
        {
            _observers.ForEach(o => o.OnDoubleRootsEvaluated());
        }
    }
}
