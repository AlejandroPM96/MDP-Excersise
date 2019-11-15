using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;

using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

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

        public void evalGrid(){
            for(int i = 0; i<this.grid.GetLength(0);i++){
                for(int j = 0; j<this.grid.GetLength(1);j++){
                    if(this.grid[i,j].value == -10 || this.grid[i,j].value == 20 || this.grid[i,j].value == 10){
                        this.grid[i,j].value = this.grid[i,j].value;
                    }else{
                        newValue(i,j);
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

        static void Main(string[] args)
        {

            Console.WriteLine("stuff");
            MDPGrid grid = new MDPGrid(5,5);
            Position[] goalPositions = new Position[2]{new Position(0,3),new Position(4,4)};
            Position[] wallPositions = new Position[4]{new Position(1,0),new Position(1,4),new Position(2,2),new Position(3,3)};
            int[] goalValues = new int[2]{10,20};

            grid.setWalls(wallPositions);
            grid.setGoals(goalPositions, goalValues);
            Console.WriteLine(grid.drawGrid());
            grid.evalGrid();
            Console.WriteLine(grid.drawGrid());
            Console.WriteLine(grid.drawGridDirection());
        

            
        }
    }
}
