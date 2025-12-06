import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { KENDO_BUTTONS } from '@progress/kendo-angular-buttons';
import { KENDO_INPUTS } from '@progress/kendo-angular-inputs';
import { KENDO_LABELS } from '@progress/kendo-angular-label';
import { PostsService } from '../posts-service/posts';
import { UserService } from '../user-service/user';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-posts',
  imports: [KENDO_LABELS, KENDO_INPUTS, KENDO_BUTTONS, CommonModule, FormsModule],
  templateUrl: './posts.html',
  styleUrl: './posts.css',
})
export class Posts {
  public vulnerableMode: boolean = false;
  public posts: any[] = [];
  public selectedPost: any = null;
  public loading: boolean = false;
  public userId: number = 0;
  public currentUserId: number | undefined = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private postsService: PostsService,
    private userService: UserService
  ) {}

  public ngOnInit(): void {
    this.getCurrentUserId();

    this.userId = Number(this.route.snapshot.paramMap.get('id'));
    this.vulnerableMode = this.route.snapshot.data['vulnerable'] === true;
    this.loadPosts();
  }

  public async loadPosts(): Promise<void> {
    this.selectedPost = null;
    this.posts = [];
    this.loading = true;

    if (this.userId !== undefined) {
      const result = await this.postsService.fetchUserPosts(
        this.userId,
        this.vulnerableMode
      );
      this.posts = result ? result.posts : [];
    }
  }

  public async getCurrentUserId(): Promise<void> {
    const user = await this.userService.getUser();
    this.currentUserId = user ? user.id : undefined;
  }

  public switchMode(): void {
    const target = this.vulnerableMode
      ? `/user/${this.userId}/posts`
      : `/v/user/${this.userId}/posts`;

    this.router.navigate([target]);
  }
}
