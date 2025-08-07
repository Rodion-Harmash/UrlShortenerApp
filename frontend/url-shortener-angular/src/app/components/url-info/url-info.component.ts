import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UrlService } from '../../services/url.service';
import { ShortUrlDto } from '../../models/short-url.dto';

@Component({
  selector: 'app-url-info',
  templateUrl: './url-info.component.html',
  styleUrls: ['./url-info.component.css']
})
export class UrlInfoComponent implements OnInit, OnDestroy {
  url: ShortUrlDto | null = null;
  error: string | null = null;
  copySuccess: boolean = false;
  private id: number = 0;
  private intervalId: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private urlService: UrlService
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    if (!this.id || isNaN(this.id)) {
      this.error = 'Invalid URL ID.';
      return;
    }

    this.fetchUrl();

    this.intervalId = setInterval(() => {
      this.fetchUrl(); // автооновлення кожні 5 сек
    }, 5000);
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  private fetchUrl(): void {
    this.urlService.getById(this.id).subscribe({
      next: (data: ShortUrlDto) => {
        this.url = data;
      },
      error: (err: any) => {
        this.error = 'Failed to load URL info.';
        console.error(err);
      }
    });
  }

  copyToClipboard(): void {
    if (!this.url) return;

    const fullLink = `http://localhost:5020/s/${this.url.shortCode}`;
    navigator.clipboard.writeText(fullLink).then(() => {
      this.copySuccess = true;
      setTimeout(() => {
        this.copySuccess = false;
      }, 1500);
    });
  }

  goBack(): void {
    this.router.navigate(['/urls']);
  }
}
