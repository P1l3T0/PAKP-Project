import { Injectable } from '@angular/core';
import { HttpService } from '../@backend/services/http.service';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PostsService {
  constructor(private http: HttpService, private router: Router) {}

  public async fetchUserPosts(id: number, vulnerable: boolean): Promise<any> {
    try {
      const posts = await firstValueFrom(this.http.fetchUserPosts(id, vulnerable));
      return posts;
    } catch (err) {
      console.error('Error fetching posts:', err);
      return null;
    }
  }

  public async getPostById(id: number, vulnerable: boolean): Promise<any> {
    try {
      const posts = await firstValueFrom(this.http.getPostById(id, vulnerable));
      return posts;
    } catch (err) {
      console.error('Error fetching post:', err);
      return null;
    }
  }

   public async deletePost(id: number): Promise<any> {
    try {
      const posts = await firstValueFrom(this.http.deletePostVuln(id));
      return posts;
    } catch (err) {
      console.error('Error deleting post:', err);
      return null;
    }
  }
}
