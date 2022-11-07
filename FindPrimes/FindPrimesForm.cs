namespace FindPrimes
{
    public partial class FindPrimesForm : Form
    {
        // Used to enable cancelation of the sync task
        private bool Canceled { get; set; }
        private bool[] fibonaccis; // Array used to determine fibonaccis

        // Fibonacci variables
        private long n1 = 0; // Initialize first Fibonacci number
        private long n2 = 1; // Initialize second Fibonacci number
        private int count = 1; // Current Fibonacci number to display 

        public FindPrimesForm()
        {
            InitializeComponent();
            progressBar.Minimum = 2;
            percentageLabel.Text = $"{0:PO}";
        }

        // Start an async Task to calculate specified Fibonacci number
        private async void calculateButton_Click(object sender, EventArgs e)
        {
            // Retrieve user's input as an integer
            long number = int.Parse(maxValueTextBox.Text);

            statusLabel.Text = "Calculating...";

            // Task to perform Fibonacci calculation in separate thread
            Task<long> fibonacciTask = Task.Run(() => Fibonacci(number));

            // Wait for Task in separate thread to complete
            await fibonacciTask;

            // Display result after Task in separate thread completes
            statusLabel.Text = fibonacciTask.Result.ToString();
        }

        // Displays Fibonacci numbers in primesTextBox
        private async Task<long> FindFibonaccis(long maximum)
        {
            var fibonacciCount = 0;

            // Find primes less than maximum
            for (var i = 0; i < maximum && !Canceled; ++i)
            {
                // If i is prime, display it
                if (await Task.Run(() => IsFibonacci(i)))
                {
                    ++fibonacciCount;
                    primesTextBox.AppendText($"{i}{Environment.NewLine}");
                }

                var percentage = (double)progressBar.Value / (progressBar.Maximum - progressBar.Minimum + 1);
                percentageLabel.Text = $"{percentage:PO}";
                progressBar.Value = i + 1;
            }

            if (Canceled)
            {
                primesTextBox.AppendText($"Canceled{Environment.NewLine}");
            }

            getPrimesButton.Enabled = true;
            cancelButton.Enabled = false;
            return fibonacciCount;
        }

        // Algorithm for calculating Fibonaccis
        public bool IsFibonacci(long value)
        {
            if (fibonaccis[value])
            {
                for (var i = value + value; i < fibonaccis.Length; i += value)
                {
                    fibonaccis[i] = false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        // Calculates nth Fibonacci number recursively
        public long Fibonacci(long n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }
            else
            {
                return Fibonacci(n - 1) + Fibonacci(n - 2);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Canceled = true;
            getPrimesButton.Enabled = true;
            cancelButton.Enabled = false;
        }
    }
}