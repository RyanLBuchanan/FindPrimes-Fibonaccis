namespace FindPrimes
{
    public partial class FindPrimesForm : Form
    {
        // Used to enable cancelation of the sync task
        private bool Canceled { get; set; }
        private bool[] primes; // Array used to determine primes

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

        // Handles getPrimesButton's click event
        private async void getPrimesButton_Click(object sender, EventArgs e)
        {
            // Get user input
            var maximum = int.Parse(maxValueTextBox.Text);

            // Create array for determining primes 
            primes = Enumerable.Repeat(true, maximum).ToArray();

            // Reset Canceled and GUI
            Canceled = false;
            getPrimesButton.Enabled = false;
            cancelButton.Enabled = true;
            primesTextBox.Text = string.Empty;
            statusLabel.Text = string.Empty;
            percentageLabel.Text = $"{0:PO}";
            progressBar.Value = progressBar.Minimum;
            progressBar.Maximum = maximum;

            // Show primes up to maximum
            int count = await FindPrimes(maximum);
            statusLabel.Text = $"Found {count} prime(s)";
        }

        // Start an async Task to calculate specified Fibonacci number
        private async void calculateButton_Click(object sender, EventArgs e)
        {
            // Retrieve user's input as an integer
            int number = int.Parse(maxValueTextBox.Text);

            statusLabel.Text = "Calculating...";

            // Task to perform Fibonacci calculation in separate thread
            Task<long> fibonacciTask = Task.Run(() => Fibonacci(number));

            // Wait for Task in separate thread to complete
            await fibonacciTask;

            // Display result after Task in separate thread completes
            statusLabel.Text = fibonacciTask.Result.ToString();
        }

        // Displays prime numbers in primesTextBox
        private async Task<int> FindPrimes(int maximum)
        {
            var primeCount = 0;

            // Find primes less than maximum
            for (var i = 2; i < maximum && !Canceled; ++i)
            {
                // If i is prime, display it
                if (await Task.Run(() => IsPrime(i)))
                {
                    ++primeCount;
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
            return primeCount;
        }

        // The Sieve of Eratosthenes
        public bool IsPrime(int value)
        {
            if (primes[value])
            {
                for (var i = value + value; i < primes.Length; i += value)
                {
                    primes[i] = false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        // Calculate next Fibonacci number iteratively 
        private void nextNumberButton_Click(object sender, EventArgs e)
        {
            // Calculate the next Fibonacci number 
            long temp = n1 + n2;
            n1 = n2;
            n2 = temp;
            ++count;

            //displayLabel.Text = $"Fibonacci of {count}:";
            //syncResultLabel.Text = n2.ToString();
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