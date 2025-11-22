import { HttpClient, HttpParams } from "@angular/common/http";
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
    return this.httpClient.post(`${this.url}${endpoint}`, body, { responseType: 'text', withCredentials: true });
  }

  public getUser(): Observable<any> {
    return this.httpClient.get(`${this.url}/api/User/get-user`, { withCredentials: true });
  }

  public fetchUsers(search: string, vulnerable: boolean): Observable<any> {
    const endpoint = vulnerable
      ? `${this.url}/api/user/search-users-vulnerable`
      : `${this.url}/api/user/search-users-safe`;

    let params = new HttpParams().set("search", search);

    return this.httpClient.get(endpoint, {
      params: params,
      withCredentials: true
    });
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

  public loginUser(email: string, password: string, vulnerable: boolean): Observable<any> {
    const endpoint = vulnerable
      ? "/api/Auth/login-vulnerable"
      : "/api/Auth/login-safe";

    return this.post(endpoint, {
      email: email,
      password: password
    });
  }

  public logoutUser(): Observable<any> {
    return this.post("/api/Auth/logout", {});
  }
}
