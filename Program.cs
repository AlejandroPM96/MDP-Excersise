using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
// using CsvHelper;

using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace MDPExcersise
{
    class Position{
        public int x {get; set;}
        public int y {get; set;}

        public Position(int x, int y){
            this.x = x;
            this.y = y;
        }
        public Position(){
            this.x = 0;
            this.y = 0;
        }
    }
    class Node{
        public double value;
        public string direction;
        public int x {get; set;}
        public int y {get; set;}

        public Node(int x, int y){
            this.x = x;
            this.y = y;
            this.value = -0.04;
            this.direction = "R";
        }

        public override string ToString(){
            return "Cell("+this.x+","+this.y+")"+" "+value;
        }
    }
    
    class MDPGrid{
        public Node[,] grid;
        public double alpha = 0.9;

        public List<List<object>> history = new List<List<object>>();
        public List<List<object>> directionHistory = new List<List<object>>();

        public IDictionary<int, string> dict = new Dictionary<int, string>();
        
        public MDPGrid(int width, int height){
            this.grid = new Node[width,height];
            for(int i = 0; i<grid.GetLength(0);i++){
                for(int j = 0; j<grid.GetLength(1);j++){
                    this.grid[i,j] = new Node(i,j);
                }
            }
            this.dict.Add(0,"R");
            this.dict.Add(1,"UR");
            this.dict.Add(2,"U");
            this.dict.Add(3,"UL");
            this.dict.Add(4,"L");
            this.dict.Add(5,"DL");
            this.dict.Add(6,"D");
            this.dict.Add(7,"DR");
            this.dict.Add(9,"S");
        }
        public void setWalls(Position[] wallPositions){
            for(int i = 0 ; i<wallPositions.GetLength(0);i++){
                this.grid[wallPositions[i].x,wallPositions[i].y].value = -10;
            }
        }

        public void setGoals(Position[] goalsPositions, int[] goalValues){
            for(int i = 0 ; i<goalsPositions.GetLength(0);i++){
                this.grid[goalsPositions[i].x,goalsPositions[i].y].value = goalValues[i];
            }
        }

        public double checkBounds(int x, int y){
            if(x<0 || y<0){
                return -10;
            }
            if(x >= this.grid.GetLength(0) || y >= this.grid.GetLength(1)){
                return 0;
            }
            return this.grid[x,y].value;
        }
        public double MyMax(params double[] values)
        {
            return values.Max();
        }
        public string getDirection(params double[] values){
            double maxValue = values.Max();
            int maxIndex = values.ToList().IndexOf(maxValue);
            return this.dict[maxIndex];
        }
        public void newValue(int i, int j){//example 0,2
            double cellValue = this.grid[i,j].value + this.alpha*MyMax(
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.9 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.9 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.9 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.9 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.9 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.9 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.9 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.9,
                checkBounds(i,j)*.9 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01
            );
            String direction = getDirection(
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.9 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.9 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.9 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.9 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.9 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.9 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.9 + checkBounds(i+1,j+1)*.01,
                checkBounds(i,j)*.01 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.9,
                checkBounds(i,j)*.9 + checkBounds(i,j+1)*.01 + checkBounds(i-1,j+1)*.01 + checkBounds(i-1,j)*.01 + checkBounds(i-1,j-1)*.01 + checkBounds(i,j-1)*.01 + checkBounds(i+1,j-1)*.01 + checkBounds(i+1,j)*.01 + checkBounds(i+1,j+1)*.01
            );
            this.grid[i,j].direction = direction;
            this.grid[i,j].value = cellValue;
        }
        public void saveState(int ic, int jc){
            List<object> newentry = new List<object>();
            List<object> newentryDirection = new List<object>();
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    newentry.Add(this.grid[i,j].value);
                    newentryDirection.Add(this.grid[i,j].direction);
                }
                this.history.Add(newentry);
                this.directionHistory.Add(newentryDirection);
                newentry = new List<object>();
                newentryDirection = new List<object>();
            }
            newentry = new List<object>(){"value",this.grid[ic,jc],"-","-","-","-","-","-"};
            this.history.Add(newentry);
            this.directionHistory.Add(newentry);
        }
        public void evalGrid(){
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    if(this.grid[i,j].value == -10 || this.grid[i,j].value == 20 || this.grid[i,j].value == 10){
                        this.grid[i,j].value = this.grid[i,j].value;
                    }else{
                        newValue(i,j);
                        saveState(i,j);
                    }
                }
            }
        }
        public string drawGrid(){
            string grid="";
            grid += "----------------------------------------------\n";
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    grid+= string.Format("{0:N2}",this.grid[i,j].value) + "\t|";
                }
                grid += "\n";
                grid += "----------------------------------------------\n";
            }
            
            return grid;
        }

        public string drawGridDirection(){
            string grid="";
            grid += "----------------------------------------------\n";
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    grid+= string.Format("{0:N2}",this.grid[i,j].direction) + "\t|";
                }
                grid += "\n";
                grid += "----------------------------------------------\n";
            }
            
            return grid;
        }

        public override string ToString(){
            string r="";
            for(int i = 0; i<grid.GetLength(0);i++){
                for(int j = 0; j<grid.GetLength(1);j++){
                    r+= this.grid[i,j].ToString() + "\n";
                }
            }
            return r;
        }
    }
    class Program
    {
        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.DisplayName); // header
                    output.Write("\t");
                }
                output.WriteLine();
                foreach (T item in data)
                {
                    foreach (PropertyDescriptor prop in props)
                    {
                        output.Write(prop.Converter.ConvertToString(
                            prop.GetValue(item)));
                        output.Write("\t");
                    }
                    output.WriteLine();
                }
            }
            static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
            static readonly string ApplicationName = "Dot Tutorials";
            static readonly string sheet = "sheetsAPI";
            static readonly string SpreadsheetId = "1zooAOBAVr90wHI-CGAtAa_UG6fwdvXsacuPBPFJc5gE";
            static SheetsService service;
        static void Init(){
            GoogleCredential credential;
            //Reading Credentials File...
            using (var stream = new FileStream("app_client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }
            // Creating Google Sheets API service...
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
        static void AddRow(MDPGrid grid) { 
            // Specifying Column Range for reading...
            var range = $"{sheet}!A:N";
            var valueRange = new ValueRange();
            // Data for another Student...
            var oblist = new List<object>() { "Harry", "80", "77", "62", "98" };
            foreach (var row in grid.history)
            {
                oblist = row;
                valueRange.Values = new List<IList<object>> { oblist };
                // Append the above record...
                var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendReponse = appendRequest.Execute();
            }

            //direction printing
            // Specifying Column Range for reading...
            // range = $"{sheet}!P:Z";
            // valueRange = new ValueRange();
            // // Data for another Student...
            // oblist = new List<object>() { "Harry", "80", "77", "62", "98" };
            // foreach (var row in grid.directionHistory)
            // {
            //     oblist = row;
            //     valueRange.Values = new List<IList<object>> { oblist };
            //     // Append the above record...
            //     var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            //     appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            //     var appendReponse = appendRequest.Execute();
            // }
            
        }
        static String makeCSV(MDPGrid grid) {
            string csv = "";
            foreach (var row in grid.history)
            {
                foreach (var cell in row)
                {
                    csv += cell+",";
                }
                csv.Remove(csv.Length - 1);
                csv += "\n";
            }
            return csv;
        }
        static String makeCSVDirs(MDPGrid grid) {
            string csv = "";
            foreach (var row in grid.directionHistory)
            {
                foreach (var cell in row)
                {
                    csv += cell+",";
                }
                csv.Remove(csv.Length - 1);
                csv += "\n";
            }
            return csv;
        }

        static void Main(string[] args)
        {

            Console.WriteLine("stuff");
            MDPGrid grid = new MDPGrid(10,15);
            Position[] goalPositions = new Position[2]{new Position(9,9),new Position(0,14)};
            Position[] wallPositions = new Position[13]{new Position(1,2),new Position(3,2),new Position(5,2),new Position(7,2),new Position(9,2),new Position(0,5),new Position(2,5),new Position(4,5),new Position(6,5),new Position(8,5),new Position(1,9),new Position(3,9),new Position(5,9)};
            int[] goalValues = new int[2]{10,20};

            grid.setWalls(wallPositions);
            grid.setGoals(goalPositions, goalValues);
            Console.WriteLine(grid.drawGrid());
            Console.WriteLine(grid.drawGridDirection());
            grid.evalGrid();
            Console.WriteLine(grid.drawGrid());
            Console.WriteLine(grid.drawGridDirection());
        
            // Init();
            // AddRow(grid);
            File.WriteAllText("valuedata.csv", makeCSV(grid));
            File.WriteAllText("directiondata.csv", makeCSVDirs(grid));
            
        }
    }
}
