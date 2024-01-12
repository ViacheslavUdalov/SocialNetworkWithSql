import {Component, OnInit} from '@angular/core';
import {MembersService} from "../../_services/members.service";
import {ActivatedRoute} from "@angular/router";
import {Member} from "../../_models/member";
import {NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions} from "@kolkov/ngx-gallery";

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit{
  member: Member
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
constructor(private  memberService: MembersService, private route: ActivatedRoute) {
}

  ngOnInit(): void {
    this.loadMember();
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

  }
  getImages():NgxGalleryImage[] {
  const imageUrls = [];
  for (const photo of this.member.photos) {
    imageUrls.push({
      small: photo?.url,
      medium: photo?.url,
      big: photo?.url
    })
  }
  return imageUrls;
  }
  loadMember() {
  this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(member => {
    this.member = member;
    // делаем это именно здесь, потому что если сделать в oninit, то будет выдавать ошибку, что photos undefined
    this.galleryImages = this.getImages();
  })
  }

}
