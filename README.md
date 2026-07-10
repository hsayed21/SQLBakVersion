# SQL Bak Version

SQL Bak Version analyzes SQL Server backup files (.bak) and database files (.mdf) to determine their SQL Server version without a preinstalled SQL Server instance. This is useful for identifying a file's version without restoring or attaching it.

## Features
- Analyze SQL Server backup (.bak) and database (.mdf) files.
- Determine the SQL Server version based on specific byte patterns within the file.
- Display the detected SQL Server version in a user-friendly manner.


## How It Works
SQL Bak Version reads the binary content of the SQL Server backup file and analyzes specific byte patterns to determine the SQL Server version. By interpreting these byte sequences, it extracts the version information without the need for a preinstalled SQL Server instance.


## Contributing
Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request.

## License
This project is licensed under the [MIT License](LICENSE.txt) - see the LICENSE file for details.

## Acknowledgements
This project was inspired by the need to quickly identify the SQL Server version of backup files without the need for a preinstalled SQL Server instance. Special thanks to the contributors who helped improve the tool.
