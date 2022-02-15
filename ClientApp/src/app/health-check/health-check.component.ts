//импорт модулей которые нужны в классе для реализации логики компонента
import { Component, Inject, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Component({
  selector: 'app-health-check',
  templateUrl: './health-check.component.html',
  styleUrls: ['./health-check.component.css']
})

//реализовал интерфейс OnInit (обеспечет type-safety)
export class HealthCheckComponent implements OnInit {
  public result: Result;

  //http - содержит клиент через который будут request/response http
  //baseUrl - using DI (dependency injection) value baseUrl (BASE_URL provider, main.ts)
  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) { }

  //this.http.get<Result>(this.baseUrl + 'hc') - create http get request to middleware ASP.Net Core
  //subscribe() - получит результат в callback в виде объекта
  ngOnInit() {
    this.http.get<Result>(this.baseUrl + 'hc').subscribe(result => {
      this.result = result;
    }, error => console.error(error));
  }
}

//описал структуру JSON, формат которой планирую получить от сервера 
interface Result {
  checks: Check[];
  totalStatus: string,
  totalResponseTime: number;
}

interface Check {
  name: string,
  status: string;
  responseTime: number
}

