import { HttpClient } from "@angular/common/http";
import { Observable} from "rxjs";
import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment.development";

@Injectable({
  providedIn: "root",
})
export class HttpService {
  private readonly url: string = environment.apiURL;

  constructor(private httpClient: HttpClient) { }

  private post(endpoint: string, body: any) {
    return this.httpClient.post(`${this.url}${endpoint}`, body, {responseType: 'text'});
  }

  public getUser(): Observable<any> {
    return this.httpClient.get(`${this.url}/api/User/get-user`);
  }

  public registerUser(
    email: string,
    username: string,
    password: string
  ): Observable<any> {
    return this.post("/api/Auth/register", {
      email: email,
      username: username,
      password: password
    });
  }

  public loginUser(email: string, password: string): Observable<any> {
    return this.post("/api/Auth/login-safe", {
      email: email,
      password: password,
    });
  }

  public loginUserVuln(email: string, password: string): Observable<any> {
    return this.post("/api/Auth/login-vulnerable", {
      email: email,
      password: password,
    });
  }

  public logoutUser(): Observable<any> {
    return this.post("/api/Auth/logout", {});
  }
}
