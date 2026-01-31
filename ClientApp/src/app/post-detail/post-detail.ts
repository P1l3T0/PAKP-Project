import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PostsService } from '../posts-service/posts';
import { KENDO_LABELS } from '@progress/kendo-angular-label';
import { KENDO_INPUTS } from '@progress/kendo-angular-inputs';
import { KENDO_BUTTONS } from '@progress/kendo-angular-buttons';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../user-service/user';

@Component({
  selector: 'app-post-detail',
  imports: [KENDO_LABELS, KENDO_INPUTS, KENDO_BUTTONS, CommonModule, FormsModule],
  templateUrl: './post-detail.html',
  styleUrl: './post-detail.css',
})
export class PostDetail {
  public postId!: number;
  public vulnerableMode: boolean = false;
  public post: any;
  public currentUserId: number | undefined = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private postsService: PostsService,
    private userService: UserService
  ) {}

  public ngOnInit(): void {
    this.getCurrentUserId();

    this.postId = Number(this.route.snapshot.paramMap.get('id'));
    this.vulnerableMode = this.route.snapshot.data['vulnerable'] === true;
    this.loadPost();
  }

  public async loadPost(): Promise<void> {
    const result = await this.postsService.getPostById(this.postId, this.vulnerableMode);
    this.post = result;
  }

  public async getCurrentUserId(): Promise<void> {
    const user = await this.userService.getUser();
    this.currentUserId = user ? user.id : undefined;
  }

  public switchMode(): void {
    const target = `/post/${this.postId}`;
    this.router.navigate([target]);
  }
}
