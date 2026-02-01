import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';
import { Forum } from './forum';

describe('Forum', () => {
  let component: Forum;
  let fixture: ComponentFixture<Forum>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Forum],
    }).compileComponents();

    fixture = TestBed.createComponent(Forum);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('XSS Prevention Tests', () => {
    it('should sanitize script tags in comments', () => {
      // Arrange
      const maliciousComment = '<script>alert("XSS")</script>';
      component.commentForm.patchValue({ comment: maliciousComment });

      // Act
      component.addComment();
      fixture.detectChanges();

      // Assert
      const commentElements = fixture.debugElement.queryAll(By.css('.comment-text span'));
      expect(commentElements.length).toBe(1);

      const commentText = commentElements[0].nativeElement.textContent;
      // Angular's interpolation {{ }} automatically escapes HTML
      expect(commentText).toContain('<script>');
      expect(commentText).toContain('alert("XSS")');
      expect(commentText).toContain('</script>');

      // Verify no actual script element was created
      const scriptElements = fixture.debugElement.queryAll(By.css('script'));
      expect(scriptElements.length).toBe(0);
    });

    it('should escape img tag with onerror event', () => {
      // Arrange
      const maliciousComment = '<img src=x onerror=alert("XSS")>';
      component.commentForm.patchValue({ comment: maliciousComment });

      // Act
      component.addComment();
      fixture.detectChanges();

      // Assert
      const commentElements = fixture.debugElement.queryAll(By.css('.comment-text span'));
      const commentText = commentElements[0].nativeElement.textContent;

      // Should be displayed as text, not executed
      expect(commentText).toContain('<img');
      expect(commentText).toContain('onerror');

      // Verify no actual img element with onerror was created in comment section
      const imgElements = fixture.debugElement.queryAll(By.css('.comment-text img'));
      expect(imgElements.length).toBe(0);
    });

    it('should escape anchor tag with javascript protocol', () => {
      // Arrange
      const maliciousComment = '<a href="javascript:alert(1)">Click me</a>';
      component.commentForm.patchValue({ comment: maliciousComment });

      // Act
      component.addComment();
      fixture.detectChanges();

      // Assert
      const commentElements = fixture.debugElement.queryAll(By.css('.comment-text span'));
      const commentText = commentElements[0].nativeElement.textContent;

      // Should be displayed as text
      expect(commentText).toContain('<a href="javascript:alert(1)">');

      // Verify no actual clickable anchor with javascript: was created
      const anchorElements = fixture.debugElement.queryAll(By.css('.comment-text a'));
      expect(anchorElements.length).toBe(0);
    });

    it('should escape iframe tags', () => {
      // Arrange
      const maliciousComment = '<iframe src="javascript:alert(1)"></iframe>';
      component.commentForm.patchValue({ comment: maliciousComment });

      // Act
      component.addComment();
      fixture.detectChanges();

      // Assert
      const commentElements = fixture.debugElement.queryAll(By.css('.comment-text span'));
      const commentText = commentElements[0].nativeElement.textContent;

      expect(commentText).toContain('<iframe');

      // Verify no actual iframe was created
      const iframeElements = fixture.debugElement.queryAll(By.css('.comment-text iframe'));
      expect(iframeElements.length).toBe(0);
    });

    it('should escape event handlers in HTML attributes', () => {
      // Arrange
      const maliciousComments = [
        '<div onclick="alert(1)">Click</div>',
        '<p onmouseover="alert(1)">Hover</p>',
        '<span onload="alert(1)">Load</span>',
      ];

      // Act & Assert
      maliciousComments.forEach((comment, index) => {
        component.commentForm.patchValue({ comment });
        component.addComment();
        fixture.detectChanges();

        const commentElements = fixture.debugElement.queryAll(By.css('.comment-text span'));
        const commentText = commentElements[index].nativeElement.textContent;

        // Should contain the raw HTML as text
        const containsEventHandler =
          commentText.includes('onclick') ||
          commentText.includes('onmouseover') ||
          commentText.includes('onload');
        expect(containsEventHandler).toBeTruthy();
      });
    });

    it('should handle multiple malicious patterns in a single comment', () => {
      // Arrange
      const maliciousComment =
        '<script>alert("XSS")</script>' +
        '<img src=x onerror=alert(1)>' +
        '<a href="javascript:void(0)">link</a>';

      component.commentForm.patchValue({ comment: maliciousComment });

      // Act
      component.addComment();
      fixture.detectChanges();

      // Assert
      const commentElements = fixture.debugElement.queryAll(By.css('.comment-text span'));
      const commentText = commentElements[0].nativeElement.textContent;

      // All malicious code should be escaped and displayed as text
      expect(commentText).toContain('<script>');
      expect(commentText).toContain('<img');
      expect(commentText).toContain('<a href');

      // No executable elements should be created
      const dangerousElements = fixture.debugElement.queryAll(
        By.css(
          '.comment-text script, .comment-text img[onerror], .comment-text a[href^="javascript:"]',
        ),
      );
      expect(dangerousElements.length).toBe(0);
    });

    it('should safely render harmless HTML as text', () => {
      // Arrange
      const comment = '<p>This is just text with &lt;tags&gt;</p>';
      component.commentForm.patchValue({ comment });

      // Act
      component.addComment();
      fixture.detectChanges();

      // Assert
      const commentElements = fixture.debugElement.queryAll(By.css('.comment-text span'));
      const commentText = commentElements[0].nativeElement.textContent;

      // Should render as text, not as HTML elements
      expect(commentText).toContain('<p>');
      expect(commentText).toContain('</p>');
    });

    it('should prevent DOM-based XSS through comment manipulation', () => {
      // Arrange
      component.comments = ['Safe comment'];

      // Attempt to inject XSS through direct array manipulation
      component.comments.push('<script>alert("Direct Injection")</script>');
      fixture.detectChanges();

      // Assert
      const commentElements = fixture.debugElement.queryAll(By.css('.comment-text span'));
      expect(commentElements.length).toBe(2);

      // Verify scripts don't execute
      const scriptElements = fixture.debugElement.queryAll(By.css('script'));
      expect(scriptElements.length).toBe(0);
    });

    it('should clear comments when mode changes to prevent state-based attacks', () => {
      // Arrange
      component.commentForm.patchValue({ comment: 'Test comment' });
      component.addComment();
      expect(component.comments.length).toBe(1);

      // Act - Toggle vulnerable mode
      component.onChange();

      // Assert
      expect(component.comments.length).toBe(0);
    });

    it('should validate that comment form prevents empty submissions', () => {
      // Arrange
      component.commentForm.patchValue({ comment: '' });

      // Act
      component.addComment();
      fixture.detectChanges();

      // Assert
      expect(component.comments.length).toBe(0);
    });

    it('should ensure innerHTML is not used in template (code review test)', () => {
      // This test verifies the template doesn't use dangerous binding
      const compiled = fixture.nativeElement;
      const commentSection = compiled.querySelector('.comments-section');

      // Verify that comments are rendered through safe interpolation, not innerHTML
      expect(commentSection).toBeTruthy();

      // Check that the template uses {{ }} interpolation (checked by examining text content)
      component.commentForm.patchValue({ comment: '<b>Bold</b>' });
      component.addComment();
      fixture.detectChanges();

      const spans = compiled.querySelectorAll('.comment-text span');
      const lastSpan = spans[spans.length - 1];

      // If properly escaped, textContent should contain the raw HTML
      expect(lastSpan.textContent).toContain('<b>Bold</b>');

      // And there should be no actual <b> tag created
      const boldTags = lastSpan.querySelectorAll('b');
      expect(boldTags.length).toBe(0);
    });
  });

  describe('Comment Functionality Tests', () => {
    it('should add comment when form is valid', () => {
      // Arrange
      const comment = 'This is a normal comment';
      component.commentForm.patchValue({ comment });

      // Act
      component.addComment();

      // Assert
      expect(component.comments.length).toBe(1);
      expect(component.comments[0]).toBe(comment);
    });

    it('should reset form after successful comment submission', () => {
      // Arrange
      const comment = 'Test comment';
      component.commentForm.patchValue({ comment });

      // Act
      component.addComment();

      // Assert
      expect(component.commentForm.value.comment).toBeNull();
    });

    it('should mark form as touched when invalid submission attempted', () => {
      // Arrange
      component.commentForm.patchValue({ comment: '' });
      spyOn(component.commentForm, 'markAllAsTouched');

      // Act
      component.addComment();

      // Assert
      expect(component.commentForm.markAllAsTouched).toHaveBeenCalled();
    });
  });
});
