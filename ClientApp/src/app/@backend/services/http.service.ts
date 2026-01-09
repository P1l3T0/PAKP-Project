import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  private readonly url: string = environment.apiURL;

  constructor(private httpClient: HttpClient) {}

  private post(endpoint: string, body: any) {
    return this.httpClient.post(`${this.url}${endpoint}`, body, {
      responseType: 'text',
      withCredentials: true,
    });
  }

  public getUser(): Observable<any> {
    return this.httpClient.get(`${this.url}/api/User/get-user`, { withCredentials: true });
  }

  public fetchUsers(search: string, vulnerable: boolean): Observable<any> {
      const endpoint = `${this.url}/api/user/search-users-vulnerable`;

    let params = new HttpParams().set('search', search);

    return this.httpClient.get(endpoint, {
      params: params,
      withCredentials: true,
    });
  }

  public registerUser(email: string, username: string, password: string): Observable<any> {
    return this.post('/api/Auth/register', {
      email: email,
      username: username,
      password: password,
    });
  }

  public loginUser(email: string, password: string, vulnerable: boolean): Observable<any> {
    const endpoint = '/api/Auth/login-vulnerable' 

    return this.post(endpoint, {
      email: email,
      password: password,
    });
  }

  public logoutUser(): Observable<any> {
    return this.post('/api/Auth/logout', {});
  }

  public fetchUserPosts(id: number, vulnerable: boolean): Observable<any> {
      const endpoint = `${this.url}/api/Post/user-posts-vulnerable/${id}`;

      return this.httpClient.get(endpoint, { withCredentials: true });
  }

  public getPostById(id: number, vulnerable: boolean): Observable<any> {
    const endpoint = `${this.url}/api/Post/get-post-vulnerable/${id}`

    return this.httpClient.get(endpoint, { withCredentials: true });
  }

  public deletePostVuln(id: number): Observable<any> {
    const endpoint = `${this.url}/api/Post/delete-post-vulnerable/${id}`;

    return this.httpClient.delete(endpoint, { withCredentials: true });
  }
}
