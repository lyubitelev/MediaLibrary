import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api/api.service';
import { MediaInfoDto } from '../models/media-info';
import { Router } from '@angular/router';

@Component({
  selector: 'app-media-list',
  templateUrl: './media-list.component.html',
  styleUrls: ['./media-list.component.css']
})
export class MediaListComponent implements OnInit {
  breakpoint!: number;
  mediaInfos!: MediaInfoDto[];

  constructor(private apiService: ApiService,
              private router: Router) { }

  ngOnInit(): void {
    this.apiService.GetAllMediaAsync().subscribe(mediaInfos => {
      this.mediaInfos = mediaInfos;
    });

    this.setBreakpoint();
  }

  onResize(event: any) {
    this.setBreakpoint();
  }

  play(url: string) {
    this.router.navigate(['play', url]);
  }

  private setBreakpoint(): void {
    if (window.innerWidth >= 1400) {
      this.breakpoint = 4;
      return;
    }

    if (window.innerWidth >= 1300) {
      this.breakpoint = 3;
      return;
    }

    if (window.innerWidth >= 1000 && window.innerHeight <= 800) {
      this.breakpoint = 3;
      return;
    }

    if (window.innerWidth >= 1000 && window.innerHeight <= 600) {
      this.breakpoint = 2;
      return;
    }

    if (window.innerWidth >= 768 && window.innerHeight <= 1368) {
      this.breakpoint = 2;
      return;
    }

    if (window.innerWidth >= 280 && window.innerHeight <= 915) {
      this.breakpoint = 1;
      return;
    }

    this.breakpoint = 4;
  }
}
