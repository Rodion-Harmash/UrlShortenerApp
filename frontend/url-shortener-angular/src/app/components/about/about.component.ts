import { Component, OnInit } from '@angular/core';
import { AboutService } from '../../services/about.service';
import { AboutDto } from '../../models/about.dto';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {
  about: AboutDto | null = null;
  content: string = '';
  error: string | null = null;
  success: boolean = false;
  isAdmin: boolean = false;
  isEditing: boolean = false;

  constructor(private aboutService: AboutService) {}

  ngOnInit(): void {
    this.loadAbout();

    const role = localStorage.getItem('role');
    this.isAdmin = role === 'Admin';
  }

  loadAbout(): void {
    this.aboutService.get().subscribe({
      next: (data: AboutDto) => {
        this.about = data;
        this.content = data.content;
        this.error = null;
      },
      error: () => {
        this.about = null;
        this.error = 'Failed to load About section.';
      }
    });
  }

  startEdit(): void {
    this.isEditing = true;
    this.success = false;
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.content = this.about?.content || '';
  }

  save(): void {
    if (!this.content.trim()) {
      this.error = 'Content cannot be empty.';
      return;
    }

    this.aboutService.update(this.content).subscribe({
      next: () => {
        this.success = true;
        this.error = null;
        this.isEditing = false;
        this.loadAbout();
      },
      error: () => {
        this.success = false;
        this.error = 'Failed to update content.';
      }
    });
  }
}
