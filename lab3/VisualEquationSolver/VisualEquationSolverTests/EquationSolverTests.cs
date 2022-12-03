using System;
using Xunit;
using VisualEquationSolver;
using Moq;

namespace VisualEquationSolverTests
{
    public class EquationSolverTests
    {
        EquationSolver sut = new EquationSolver();
        Mock<IEquationSolverUiObserver> observerMock = new Mock<IEquationSolverUiObserver>();

        [Fact]
        public void Change_constant_coeff_obverver_notified_that_coeff_changed()
        {
            sut.Subscribe(observerMock.Object);

            sut.SetConstantCoeff(1);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Change_linear_coeff_obverver_notified_that_coeff_changed()
        {
            sut.Subscribe(observerMock.Object);

            sut.SetLinearCoeff(1);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Change_quadratic_coeff_obverver_notified_that_coeff_changed()
        {
            sut.Subscribe(observerMock.Object);

            sut.SetQuadraticCoeff(1);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Change_constant_coeff_when_oberver_was_unsubscribed_observer_not_notified()
        {
            var t = sut.Subscribe(observerMock.Object);

            sut.SetConstantCoeff(1);
            sut.Unsubscribe(t);
            sut.SetConstantCoeff(2);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Change_linear_coeff_when_oberver_was_unsubscribed_observer_not_notified()
        {
            var t = sut.Subscribe(observerMock.Object);

            sut.SetLinearCoeff(1);
            sut.Unsubscribe(t);
            sut.SetLinearCoeff(2);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Change_quadratic_coeff_when_oberver_was_unsubscribed_observer_not_notified()
        {
            var t = sut.Subscribe(observerMock.Object);

            sut.SetQuadraticCoeff(1);
            sut.Unsubscribe(t);
            sut.SetQuadraticCoeff(2);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Set_same_constant_coeff_value_observer_notified_only_once()
        {
            sut.Subscribe(observerMock.Object);

            sut.SetConstantCoeff(1);
            sut.SetConstantCoeff(1);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Set_same_linear_coeff_value_observer_notified_only_once()
        {
            sut.Subscribe(observerMock.Object);

            sut.SetLinearCoeff(1);
            sut.SetLinearCoeff(1);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Set_same_quadratic_coeff_value_observer_notified_only_once()
        {
            sut.Subscribe(observerMock.Object);

            sut.SetQuadraticCoeff(1);
            sut.SetQuadraticCoeff(1);

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Once());
        }

        [Fact]
        public void Get_roots_when_quadratic_coeff_is_zero_exception_thrown()
        {
            Assert.Throws<InvalidOperationException>(sut.GetRoots);
        }

        [Fact]
        public void Get_roots_when_quadratic_coeff_is_zero_observer_wasnt_notified()
        {
            sut.Subscribe(observerMock.Object);

            Assert.Throws<InvalidOperationException>(sut.GetRoots);

            observerMock.Verify(obs => obs.OnDoubleRootsEvaluated(), Times.Never());
            observerMock.Verify(obs => obs.OnSingleRootEvaluated(), Times.Never());
            observerMock.Verify(obs => obs.OnNoRootsEvaluated(), Times.Never());
        }

        [Fact]
        public void Get_roots_of_equation_with_no_solutions_observer_notified_about_no_roots()
        {
            sut.Subscribe(observerMock.Object);
            sut.SetQuadraticCoeff(2);
            sut.SetLinearCoeff(3);
            sut.SetConstantCoeff(4);

            var roots = sut.GetRoots();

            observerMock.Verify(obs => obs.OnNoRootsEvaluated(), Times.Once());
            Assert.Empty(roots);
        }

        [Fact]
        public void Get_roots_of_equation_with_one_solutions_observer_notified_about_single_root_and_root_is_correct()
        {
            sut.Subscribe(observerMock.Object);
            sut.SetQuadraticCoeff(2);
            sut.SetLinearCoeff(4);
            sut.SetConstantCoeff(2);

            var roots = sut.GetRoots();

            observerMock.Verify(obs => obs.OnSingleRootEvaluated(), Times.Once());
            Assert.Single(roots);
            Assert.Equal(-1, roots[0]);
        }

        [Fact]
        public void Get_roots_of_equation_with_two_solutions_observer_notified_about_double_roots_and_roots_are_correct()
        {
            sut.Subscribe(observerMock.Object);
            sut.SetQuadraticCoeff(2);
            sut.SetLinearCoeff(4);
            sut.SetConstantCoeff(-6);

            var roots = sut.GetRoots();

            observerMock.Verify(obs => obs.OnDoubleRootsEvaluated(), Times.Once());
            Assert.Equal(2, roots.Count);
            Assert.Equal(1, roots[0]);
            Assert.Equal(-3, roots[1]);
        }

        [Fact]
        public void Get_roots_twice_with_same_coeffs_roots_evaluated_only_once_results_are_same()
        {
            sut.Subscribe(observerMock.Object);
            sut.SetQuadraticCoeff(2);
            sut.SetLinearCoeff(4);
            sut.SetConstantCoeff(-6);

            var firstResult = sut.GetRoots();
            var secondResult = sut.GetRoots();

            observerMock.Verify(obs => obs.OnDoubleRootsEvaluated(), Times.Once());
            Assert.StrictEqual(firstResult, secondResult);
        }

        [Fact]
        public void Get_roots_after_coeff_was_set_to_same_value_roots_evaluated_only_once_results_are_same()
        {
            sut.Subscribe(observerMock.Object);
            sut.SetQuadraticCoeff(2);
            sut.SetLinearCoeff(4);
            sut.SetConstantCoeff(-6);

            var firstResult = sut.GetRoots();
            sut.SetQuadraticCoeff(2);
            var secondResult = sut.GetRoots();

            observerMock.Verify(obs => obs.OnCoeffChanged(), Times.Exactly(3));
            observerMock.Verify(obs => obs.OnDoubleRootsEvaluated(), Times.Once());
            Assert.StrictEqual(firstResult, secondResult);
        }
    }
}
