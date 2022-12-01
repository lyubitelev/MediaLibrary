import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MediaListComponent } from './media-list/media-list.component';
import { MediaPlayerComponent } from './media-player/media-player.component';

const routes: Routes = [
  { path: '', component: MediaListComponent },
  { path: 'play/:id',    component: MediaPlayerComponent },
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
